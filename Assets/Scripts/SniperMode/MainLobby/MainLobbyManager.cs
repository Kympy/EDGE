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
        CreateButton.onClick.AddListener(() => CreateUI()); // Show Create UI button
        ExitButton.onClick.AddListener(() => CloseCreateUI()); // Close UI
        RealCreateButton.onClick.AddListener(() => CreateRoom()); // Real Room Create
        LockedRoom.onValueChanged.AddListener(delegate { PassInput.interactable = LockedRoom.isOn; 
        if(LockedRoom.isOn == false)
            {
                PassInput.text = ""; // Can only input password when lock toggle is on
            }
        });
        BetAmountInput.interactable = false;
        CreateCanvas.gameObject.SetActive(false); // Hide when start
        // ======== User Info UI ====================================================
        CloseButton.onClick.AddListener(delegate { UserInfoCanvas.SetActive(false); }); // Close Button
        ShowButton.onClick.AddListener(delegate { UserInfoCanvas.SetActive(true); }); // Show Button
        UserInfoCanvas.SetActive(false); // Hide when start
        // ======== Init Bool Value =================================================
        for(int i = 0; i < MAX_ROOM; i++)
        {
            IsRoomExist[i] = false;
        }
        GameMode.value = 0;
        CreateButton.interactable = false;
    }
    private IEnumerator Start() // Get Datas And Update UI
    {
        UserName.text = ODINAPIHandler.Instance.GetUserProfile().userProfile.username; // Set profile UI
        PopUserName.text = ODINAPIHandler.Instance.GetUserProfile().userProfile.username;
        UserEmail.text = ODINAPIHandler.Instance.GetUserProfile().userProfile.email_id;
        yield return ODINAPIHandler.Instance.ProcessGetBalance(ODINAPIHandler.COIN_TYPE.ace); // Wait for getting balances
        yield return ODINAPIHandler.Instance.ProcessGetBalance(ODINAPIHandler.COIN_TYPE.zera);
        yield return ODINAPIHandler.Instance.ProcessGetBalance(ODINAPIHandler.COIN_TYPE.dappx);
        ACEBalance.text = ODINAPIHandler.Instance.GetBalance(ODINAPIHandler.COIN_TYPE.ace).Value.data.balance; // Set UI
        ACEBalance.text = float.Parse(ACEBalance.text).ToString("0.00");
        ZERABalance.text = ODINAPIHandler.Instance.GetBalance(ODINAPIHandler.COIN_TYPE.zera).Value.data.balance;
        ZERABalance.text = float.Parse(ZERABalance.text).ToString("0.00");
        DAPPXBalance.text = ODINAPIHandler.Instance.GetBalance(ODINAPIHandler.COIN_TYPE.dappx).Value.data.balance;
        DAPPXBalance.text = float.Parse(DAPPXBalance.text).ToString("0.00");
        yield return ODINAPIHandler.Instance.ProcessGetBetSettings(); // Request Betting Settings
        CreateButton.interactable = true;
        StartCoroutine(UpdateUserCount()); // Start Updating Online user count
    }
    #region Creating Room
    private void CreateUI() // Show Create UI
    {
        CreateButton.interactable = false; // Lock Buttons
        CreateCanvas.gameObject.SetActive(true); // Show UI
        BetAmountInput.text = ODINAPIHandler.Instance.GetBetSettings().data.bets[0].amount.ToString();
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
    private void CreateRoom()
    {
        ExitGames.Client.Photon.Hashtable custom = new ExitGames.Client.Photon.Hashtable();
        custom.Add("Mode", GameMode.options[GameMode.value].text);
        custom.Add("Bet", ODINAPIHandler.Instance.GetBetSettings().data.bets[0].amount.ToString());
        custom.Add("RoomName", RoomNameInput.text);
        custom.Add("Password", PassInput.text);
        string[] forLobbyCustom = { "Mode", "Bet", "RoomName", "Password"};
        RoomOptions op = new RoomOptions();
        op.IsOpen = true;
        op.IsVisible = true;
        op.MaxPlayers = 2;
        op.CustomRoomPropertiesForLobby = forLobbyCustom;
        op.CustomRoomProperties = custom;
        //ODINAPIHandler.Instance.GetBetSettings();
        PhotonNetwork.CreateRoom(PhotonNetwork.NetworkingClient.UserId, op);
    }
    #endregion
    public override void OnJoinedRoom() // When joined room
    {
        PhotonNetwork.LoadLevel(2); // Load level 2 (room scene)
    }
    #region Get Room List
    public override void OnRoomListUpdate(List<RoomInfo> roomList) // Call rate = 5 sec
    {
        RoomCount.text = PhotonNetwork.CountOfRooms.ToString() + " Rooms"; // Show Room Count
        foreach(RoomInfo room in roomList) // Check each room
        {
            if (room.RemovedFromList) // Is removed Room?
            {
                TotalRoomList.TryGetValue(room.Name, out GameObject roomObj); // Search in my room list
                Destroy(roomObj); // Destroy that gameobject
                TotalRoomList.Remove(room.Name); // Remove from list
            }
            else  // Exist something change
            {
                if (TotalRoomList.ContainsKey(room.Name) == false) // Search in my room list -> I don't have new room info
                {
                    // Get Custom properties information
                    if (room.CustomProperties.TryGetValue("Mode", out object modeName))
                    {
                        bool isLocked = false;
                        room.CustomProperties.TryGetValue("Password", out object password);
                        if (password.ToString() == "")
                        {
                            isLocked = false;
                        }
                        else isLocked = true;

                        GameObject newRoom = null; // For create new room object prefab

                        int index = GetRoomPos(); // Where room should show?
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
                        if (mode == "Sniper Mode") // Switch prefab by mode name
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
                        TotalRoomList.Add(room.Name, newRoom); // Add new room info on my local room list
                    }
                }
                else // Already in my local list, so change property values
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
