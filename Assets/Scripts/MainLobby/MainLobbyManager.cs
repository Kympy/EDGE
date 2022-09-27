using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class MainLobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Room Info")] // Rooms
    [SerializeField] private RectTransform[] RoomPosition = new RectTransform[6];
    [SerializeField] private bool[] IsRoomExist = new bool[6]; // Is there a room
    [SerializeField] private Button CreateButton = null;
    [Header("Page Info")] // Pages
    [SerializeField] private Button NextPage = null;
    [SerializeField] private Button PreviousPage = null;
    [SerializeField] private TextMeshProUGUI PageInfo = null;
    [Header("CreateUI")]
    [SerializeField] private Canvas CreateCanvas = null;
    [SerializeField] private TMP_Dropdown GameMode = null;
    [SerializeField] private TMP_InputField RoomNameInput = null;
    [SerializeField] private TMP_InputField BetAmountInput = null;
    [SerializeField] private Button ExitButton = null;
    [SerializeField] private Button RealCreateButton = null;
    [SerializeField] private TMP_InputField PassInput = null;
    [SerializeField] private Toggle LockedRoom = null;

    private Dictionary<string, GameObject> TotalRoomList = new Dictionary<string, GameObject>();
    private GameObject SniperRoomPrefab = null;
    private GameObject PistolRoomPrefab = null;
    private GameObject DartRoomPrefab = null;

    // Drop down menu option list
    private string[] ModeList = { "Sniper Mode", "Pistol Mode", "Dart Mode" };

    private void Awake()
    {
        StartCoroutine(ReJoinLobby());
        SniperRoomPrefab = Resources.Load<GameObject>("Rooms/SniperRoom");
        PistolRoomPrefab = Resources.Load<GameObject>("Rooms/PistolRoom");
        DartRoomPrefab = Resources.Load<GameObject>("Rooms/DartRoom");
        CreateButton.onClick.AddListener(() => CreateUI());
        ExitButton.onClick.AddListener(() => CloseCreateUI());
        RealCreateButton.onClick.AddListener(() => CreateRoom());
        LockedRoom.onValueChanged.AddListener(delegate { PassInput.interactable = LockedRoom.isOn; 
        if(LockedRoom.isOn == false)
            {
                PassInput.text = "";
            }
        });
        GameMode.onValueChanged.AddListener(delegate { Debug.Log(GameMode.options[GameMode.value].text); });
        CreateCanvas.gameObject.SetActive(false);

        for(int i = 0; i < IsRoomExist.Length; i++)
        {
            IsRoomExist[i] = false;
        }
    }
    private IEnumerator ReJoinLobby()
    {
        while(true)
        {
            if(PhotonNetwork.IsConnectedAndReady)
            {
                if(PhotonNetwork.InLobby == false)
                {
                    PhotonNetwork.JoinLobby();
                    yield break;
                }
            }
            yield return null;
        }
    }
    private void CreateUI()
    {
        CreateButton.interactable = false; // Lock Buttons
        NextPage.interactable = false;
        PreviousPage.interactable = false;
        CreateCanvas.gameObject.SetActive(true); // Show UI
        // Drop down menu initializing
        GameMode.ClearOptions(); // Clear Options
        for (int i = 0; i < ModeList.Length; i++)
        {
            TMP_Dropdown.OptionData opData = new TMP_Dropdown.OptionData();
            opData.text = ModeList[i];
            opData.image = null;
            GameMode.options.Add(opData);
        }
        LockedRoom.isOn = false;
        PassInput.text = "";
    }
    private void CloseCreateUI() // Close UI
    {
        CreateButton.interactable = true; // Allow Buttons
        NextPage.interactable = true;
        PreviousPage.interactable = true;
        CreateCanvas.gameObject.SetActive(false);
    }
    private void CreateRoom()
    {
        ExitGames.Client.Photon.Hashtable custom = new ExitGames.Client.Photon.Hashtable();
        custom.Add("Mode", GameMode.options[GameMode.value].text);
        custom.Add("Bet", BetAmountInput.text);
        custom.Add("RoomName", RoomNameInput.text);
        custom.Add("Password", PassInput.text);
        Debug.Log(PassInput.text);
        string[] forLobbyCustom = { "Mode", "Bet", "RoomName", "Password"};
        RoomOptions op = new RoomOptions();
        op.IsOpen = true;
        op.IsVisible = true;
        op.MaxPlayers = 2;
        op.CustomRoomPropertiesForLobby = forLobbyCustom;
        op.CustomRoomProperties = custom;
        PhotonNetwork.CreateRoom(RoomNameInput.text, op);
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(2);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("enter");
        Debug.Log(roomList.Count);
        for(int i = 0; i < roomList.Count; i++)
        {
            Debug.Log(roomList[i].Name);
        }

        foreach(RoomInfo room in roomList)
        {
            Debug.Log("ente2r");
            if (room.RemovedFromList)
            {
                Debug.Log("three");
                TotalRoomList.TryGetValue(room.Name, out GameObject roomObj);
                Destroy(roomObj);
                TotalRoomList.Remove(room.Name);
            }
            else 
            {
                Debug.Log("asdf");
                if (TotalRoomList.ContainsKey(room.Name) == false)
                {
                    Debug.Log("Create New Room : Get Mode Value");
                    if (room.CustomProperties.TryGetValue("Mode", out object modeName))
                    {
                        bool isLocked = false;
                        room.CustomProperties.TryGetValue("Password", out object password);
                        if (password.ToString() == "")
                        {
                            Debug.Log("six");
                            isLocked = false;
                        }
                        else isLocked = true;
                        Debug.Log("seven");

                        Transform roomPos = RoomPosition[GetRoomPos()];
                        GameObject newRoom = null;
                        string mode = modeName.ToString();
                        Debug.Log("Create New Room : Get BetAmount");
                        string betAmount = "0";
                        if (room.CustomProperties.TryGetValue("Bet", out object value))
                        {
                            betAmount = value.ToString();
                            Debug.Log("New Room Bets : " + betAmount);
                        }
                        Debug.Log("one");
                        string realName = null;
                        if (room.CustomProperties.TryGetValue("RoomName", out object roomName))
                        {
                            realName = roomName.ToString();
                            Debug.Log("two");
                        }
                        if (mode == "Sniper Mode")
                        {
                            Debug.Log("Sniper Mode Room");
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
                        Debug.Log("Last");
                    }
                }
                else
                {
                    Debug.Log("enter3");
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
                    Debug.Log("enter4");
                }
            }
            Debug.Log("enter123");
        }
        Debug.Log("enter1234");
    }
    public int GetRoomPos()
    {
        for(int i = 0; i < IsRoomExist.Length; i++)
        {
            if(IsRoomExist[i] == false)
            {
                return i;
            }
        }
        return 0;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(20f, 50f, 200f, 20f), "Client State : " + PhotonNetwork.NetworkClientState.ToString());
        //GUI.Label(new Rect(20f, 70f, 200f, 20f), "LobbyName : " + PhotonNetwork.CurrentLobby.Name);
        GUI.Label(new Rect(20f, 90f, 200f, 20f), "Server State : " + PhotonNetwork.Server);
    }
}
