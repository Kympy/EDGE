using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class MainLobbyManager : MonoBehaviourPunCallbacks
{
    #region Variables
    [Header("UserInfo")]
    [SerializeField] private TextMeshProUGUI UserName = null;
    [SerializeField] private TextMeshProUGUI PopUserName = null;
    [SerializeField] private TextMeshProUGUI UserEmail = null;
    [SerializeField] private TextMeshProUGUI ACEBalance = null;
    [SerializeField] private TextMeshProUGUI ZERABalance = null;
    [SerializeField] private TextMeshProUGUI DAPPXBalance = null;
    [SerializeField] private GameObject UserInfoCanvas = null;
    [SerializeField] private Button CloseButton = null;
    [SerializeField] private Button ShowButton = null;
    [Header("Room Info")] // Rooms
    [SerializeField] private RectTransform[] RoomPosition = new RectTransform[18];
    [SerializeField] private Button CreateButton = null;
    [SerializeField] private TextMeshProUGUI UserCount = null;
    [SerializeField] private TextMeshProUGUI RoomCount = null;
    [Header("CreateUI")]
    [SerializeField] private Canvas CreateCanvas = null;
    [SerializeField] private TMP_Dropdown GameMode = null;
    [SerializeField] private TMP_InputField RoomNameInput = null;
    [SerializeField] private TMP_InputField BetAmountInput = null;
    [SerializeField] private Button ExitButton = null;
    [SerializeField] private Button RealCreateButton = null;
    [SerializeField] private TMP_InputField PassInput = null;
    [SerializeField] private Toggle LockedRoom = null;
    // Room Management
    private Dictionary<string, GameObject> TotalRoomList = new Dictionary<string, GameObject>();
    private const int MAX_ROOM = 18;
    private bool[] IsRoomExist = new bool[MAX_ROOM];

    // Room Prefabs
    private GameObject SniperRoomPrefab = null;
    private GameObject PistolRoomPrefab = null;
    private GameObject DartRoomPrefab = null;

    // Drop down menu option list
    private string[] ModeList = { "Sniper Mode", "Pistol Mode", "Dart Mode" };
    #endregion

    private void Awake()
    {
        // ========= Resource Load ================================================
        SniperRoomPrefab = Resources.Load<GameObject>("SniperMode/Rooms/SniperRoom");
        PistolRoomPrefab = Resources.Load<GameObject>("SniperMode/Rooms/PistolRoom");
        DartRoomPrefab = Resources.Load<GameObject>("SniperMode/Rooms/DartRoom");
        // ======== Create UI =====================================================
        CreateButton.onClick.AddListener(() => CreateUI());
        ExitButton.onClick.AddListener(() => CloseCreateUI());
        RealCreateButton.onClick.AddListener(() => StartCoroutine(CreateRoom()));
        LockedRoom.onValueChanged.AddListener(delegate { PassInput.interactable = LockedRoom.isOn; 
        if(LockedRoom.isOn == false)
            {
                PassInput.text = ""; // Can only input password when lock toggle is on
            }
        });
        GameMode.onValueChanged.AddListener(delegate { Debug.Log(GameMode.options[GameMode.value].text); });

        CreateCanvas.gameObject.SetActive(false); // Hide when start
        // ======== User Info UI ====================================================
        CloseButton.onClick.AddListener(delegate { UserInfoCanvas.SetActive(false); });
        ShowButton.onClick.AddListener(delegate { UserInfoCanvas.SetActive(true); });
        UserInfoCanvas.SetActive(false);
        // ======== Init Bool Value =================================================
        for(int i = 0; i < MAX_ROOM; i++)
        {
            IsRoomExist[i] = false;
        }
    }
    private IEnumerator Start()
    {
        UserName.text = ODINAPIHandler.Instance.GetUserProfile().userProfile.username; // Set profile UI
        PopUserName.text = ODINAPIHandler.Instance.GetUserProfile().userProfile.username;
        UserEmail.text = ODINAPIHandler.Instance.GetUserProfile().userProfile.email_id;
        yield return ODINAPIHandler.Instance.ProcessGetBalance(ODINAPIHandler.COIN_TYPE.ace); // Wait for getting balances
        yield return ODINAPIHandler.Instance.ProcessGetBalance(ODINAPIHandler.COIN_TYPE.zera);
        yield return ODINAPIHandler.Instance.ProcessGetBalance(ODINAPIHandler.COIN_TYPE.dappx);
        ACEBalance.text = ODINAPIHandler.Instance.GetBalance(ODINAPIHandler.COIN_TYPE.ace).Value.data.balance; // Set UI
        ZERABalance.text = ODINAPIHandler.Instance.GetBalance(ODINAPIHandler.COIN_TYPE.zera).Value.data.balance;
        DAPPXBalance.text = ODINAPIHandler.Instance.GetBalance(ODINAPIHandler.COIN_TYPE.dappx).Value.data.balance;
        StartCoroutine(UpdateUserCount()); // Start Updating Online user count
    }
    #region Creating Room
    private void CreateUI() // Show Create UI
    {
        CreateButton.interactable = false; // Lock Buttons
        CreateCanvas.gameObject.SetActive(true); // Show UI
        // Drop down menu initializing
        GameMode.ClearOptions(); // Clear Options
        for (int i = 0; i < ModeList.Length; i++)// Add Option
        {
            TMP_Dropdown.OptionData opData = new TMP_Dropdown.OptionData();
            opData.text = ModeList[i]; // Set Text
            opData.image = null; // Non image
            GameMode.options.Add(opData); // Add
        }
        LockedRoom.isOn = false; // False when start
        PassInput.text = ""; // Init
    }
    private void CloseCreateUI() // Close UI
    {
        CreateButton.interactable = true; // Allow Buttons
        CreateCanvas.gameObject.SetActive(false);
    }
    private IEnumerator CreateRoom()
    {
        ExitGames.Client.Photon.Hashtable custom = new ExitGames.Client.Photon.Hashtable();
        custom.Add("Mode", GameMode.options[GameMode.value].text);
        custom.Add("Bet", BetAmountInput.text);
        custom.Add("RoomName", RoomNameInput.text);
        custom.Add("Password", PassInput.text);
        string[] forLobbyCustom = { "Mode", "Bet", "RoomName", "Password"};
        RoomOptions op = new RoomOptions();
        op.IsOpen = true;
        op.IsVisible = true;
        op.MaxPlayers = 2;
        op.CustomRoomPropertiesForLobby = forLobbyCustom;
        op.CustomRoomProperties = custom;
        yield return ODINAPIHandler.Instance.ProcessGetBetSettings(); // Request Betting Settings
        //ODINAPIHandler.Instance.GetBetSettings();
        PhotonNetwork.CreateRoom(RoomNameInput.text, op);
    }
    #endregion
    public override void OnJoinedRoom() // When joined room
    {
        PhotonNetwork.LoadLevel(2); // Load level 2 (room scene)
    }
    #region Get Room List
    public override void OnRoomListUpdate(List<RoomInfo> roomList) // Call rate = 5 sec
    {
        RoomCount.text = PhotonNetwork.CountOfRooms.ToString() + " Rooms";
        foreach(RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                TotalRoomList.TryGetValue(room.Name, out GameObject roomObj);
                Destroy(roomObj);
                TotalRoomList.Remove(room.Name);
            }
            else 
            {
                if (TotalRoomList.ContainsKey(room.Name) == false)
                {
                    if (room.CustomProperties.TryGetValue("Mode", out object modeName))
                    {
                        bool isLocked = false;
                        room.CustomProperties.TryGetValue("Password", out object password);
                        if (password.ToString() == "")
                        {
                            isLocked = false;
                        }
                        else isLocked = true;

                        GameObject newRoom = null;

                        int index = GetRoomPos();
                        Transform roomPos = RoomPosition[index];
                        IsRoomExist[index] = true;
    
                        string mode = modeName.ToString();
                        string betAmount = "0";
                        if (room.CustomProperties.TryGetValue("Bet", out object value))
                        {
                            betAmount = value.ToString();
                        }
                        string realName = null;
                        if (room.CustomProperties.TryGetValue("RoomName", out object roomName))
                        {
                            realName = roomName.ToString();
                        }
                        if (mode == "Sniper Mode")
                        {
                            newRoom = Instantiate(SniperRoomPrefab, roomPos.position, Quaternion.identity);
                            newRoom.transform.SetParent(roomPos);
                            newRoom.GetComponent<RoomInformation>().InitRoom(realName, betAmount, isLocked, room.Name, password.ToString());
                        }
                        else if (mode == "Pistol Mode")
                        {
                            newRoom = Instantiate(PistolRoomPrefab, roomPos.position, Quaternion.identity);
                            newRoom.transform.SetParent(roomPos);
                            newRoom.GetComponent<RoomInformation>().InitRoom(realName, betAmount, isLocked, room.Name, password.ToString());
                        }
                        else if (mode == "Dart Mode")
                        {
                            newRoom = Instantiate(DartRoomPrefab, roomPos.position, Quaternion.identity);
                            newRoom.transform.SetParent(roomPos);
                            newRoom.GetComponent<RoomInformation>().InitRoom(realName, betAmount, isLocked, room.Name, password.ToString());
                        }
                        else
                        {
                            Debug.Log("Room Creation Error : Can't detect mode");
                            continue;
                        }
                        TotalRoomList.Add(room.Name, newRoom);
                    }
                }
                else
                {
                    TotalRoomList.TryGetValue(room.Name, out GameObject roomObj);
                    room.CustomProperties.TryGetValue("Bet", out object value);
                    room.CustomProperties.TryGetValue("RoomName", out object newName);
                    room.CustomProperties.TryGetValue("Password", out object pass);
                    bool isLocked = false;
                    if (pass.ToString() == "")
                    {
                        isLocked = false;
                    }
                    else isLocked = true;
                    roomObj.GetComponent<RoomInformation>().InitRoom(newName.ToString(), value.ToString(), isLocked, room.Name, pass.ToString());
                }
            }
        }
    }
    public int GetRoomPos() // Where room have to create?
    {
        for(int i = 0; i < MAX_ROOM; i++)
        {
            if (IsRoomExist[i] == false) // There's no room
            {
                return i; // return this position
            }
        }
        return 0;
    }
    #endregion
    private IEnumerator UpdateUserCount() // Show online user
    {
        WaitForSeconds UpdateTime = new WaitForSeconds(5f); // Because photon networking update rate is 5 sec
        while(true)
        {
            UserCount.text = "ONLINE : " + PhotonNetwork.CountOfPlayers.ToString(); // How many players in app
            yield return UpdateTime;
        }
    }
#if UNITY_EDITOR
    private void OnGUI()
    {
        GUI.Label(new Rect(20f, 50f, 200f, 20f), "Client State : " + PhotonNetwork.NetworkClientState.ToString());
        GUI.Label(new Rect(20f, 90f, 200f, 20f), "Server State : " + PhotonNetwork.Server);
    }
#endif
}
