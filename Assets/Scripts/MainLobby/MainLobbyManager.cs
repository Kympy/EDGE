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
    // Drop down menu option list
    private string[] ModeList = { "Sniper Mode", "Pistol Mode", "Dart Mode" };
    private string RoomName = "";
    private int BetAmount = 0;

    private void Awake()
    {
        CreateButton.onClick.AddListener(() => CreateUI());
        ExitButton.onClick.AddListener(() => CloseCreateUI());
        RealCreateButton.onClick.AddListener(() => CreateRoom());
        CreateCanvas.gameObject.SetActive(false);
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
        GameObject room = PhotonNetwork.Instantiate("Rooms/SniperRoom", RoomPosition[0].position, Quaternion.identity);
        //room.GetComponent<RoomInformation>().InitRoom(RoomNameInput.text, BetAmountInput.text);
        //PhotonNetwork.CreateRoom(RoomNameInput.text, new RoomOptions { MaxPlayers = 2 }, null);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //base.OnRoomListUpdate(roomList);
    }
    private void OnGUI()
    {
        GUI.Label(new Rect(20f, 50f, 200f, 20f), "Server State : " + PhotonNetwork.NetworkClientState.ToString());
        GUI.Label(new Rect(20f, 70f, 200f, 20f), "LobbyName : " + PhotonNetwork.CurrentLobby.Name);
    }
}
