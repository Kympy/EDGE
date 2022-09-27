using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private RectTransform User1Pos = null;
    [SerializeField] private RectTransform User2Pos = null;

    [SerializeField] private Button RoomSettingButton = null;
    [SerializeField] private Button RoomExitButton = null;

    [SerializeField] private Button EditButton = null;
    [SerializeField] private Button CancelButton = null;
    [SerializeField] private TMP_InputField NameInput = null;
    [SerializeField] private TMP_InputField BetInput = null;
    [SerializeField] private Toggle lockedRoom = null;
    [SerializeField] private TMP_InputField PassInput = null;
    [SerializeField] private GameObject EditCanvas = null;

    [SerializeField] private TextMeshProUGUI RoomTitle = null;
    [SerializeField] private TextMeshProUGUI BetAmount = null;
    private void Awake()
    {
        RoomSettingButton.onClick.AddListener(() => ToggleEditUI(true));
        CancelButton.onClick.AddListener(() => ToggleEditUI(false));
        RoomExitButton.onClick.AddListener(() => ExitRoom());
        EditButton.onClick.AddListener(() => EditRoom());
        lockedRoom.onValueChanged.AddListener(delegate { PassInput.interactable = lockedRoom.isOn;
            if (lockedRoom.isOn == false)
            {
                PassInput.text = "";
            }
        });
        EditCanvas.SetActive(false);
        lockedRoom.isOn = false;
        InitRoom();

        photonView.RPC("ShowUser", RpcTarget.AllBuffered, GetPos());
        ShowUser(GetPos());
    }
    public void InitRoom()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("RoomName", out object name))
        {
            RoomTitle.text = name.ToString();
        }
        else RoomTitle.text = "Room Name Error";

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Bet", out object value))
        {
            BetAmount.text = value.ToString();
        }
        else BetAmount.text = "Error";
    }
    private void ToggleEditUI(bool isTrue)
    {
        if(isTrue)
        {
            NameInput.text = RoomTitle.text;
            BetInput.text = BetAmount.text;
            if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Password", out object pass))
            {
                if (pass.ToString() == "")
                {
                    lockedRoom.isOn = false;
                }
                else
                {
                    lockedRoom.isOn = true;
                    PassInput.text = pass.ToString();
                }
            }
        }
        EditCanvas.SetActive(isTrue);
    }
    private void EditRoom()
    {
        //RoomOptions op = new RoomOptions();

        //op.IsVisible = true;
        //op.IsOpen = true;
        Debug.Log("Edit");
        ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;
        hash.Remove("Bet");
        hash.Remove("RoomName");
        hash.Remove("Password");
        hash.Add("Bet", BetInput.text);
        hash.Add("RoomName", NameInput.text);
        hash.Add("Password", PassInput.text);
        //op.CustomRoomProperties = hash;

        //string[] forLobbyCustom = { "Mode", "Bet", "RoomName", "Password" };
        //op.CustomRoomPropertiesForLobby = forLobbyCustom;
        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        InitRoom();
        ToggleEditUI(false);
    }
    private void ExitRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    
    public override void OnLeftRoom()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        PhotonNetwork.LoadLevel(1);
    }
    
    private int GetPos()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            return 1;
        }
        else return 2;
    }
    [PunRPC]
    public void ShowUser(int pos)
    {
        GameObject box = PhotonNetwork.Instantiate("Rooms/UserBox", Vector3.zero, Quaternion.identity);
        if (pos == 1)
        {
            box.transform.position = User1Pos.position;
            box.transform.SetParent(User1Pos);
            box.GetComponent<UserBox>().InitUserUI(PhotonNetwork.NickName, "60.0", "357", 1);
        }
        else
        {
            box.transform.position = User2Pos.position;
            box.transform.SetParent(User2Pos);
            box.GetComponent<UserBox>().InitUserUI(PhotonNetwork.NickName, "60.0", "357", 2);
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        
    }
    private void OnGUI()
    {
        GUI.Label(new Rect(20f, 50f, 200f, 20f), "Client State : " + PhotonNetwork.NetworkClientState.ToString());
        //GUI.Label(new Rect(20f, 70f, 200f, 20f), "RoomName : " + PhotonNetwork.CurrentRoom.Name);
        GUI.Label(new Rect(20f, 90f, 200f, 20f), "Server State : " + PhotonNetwork.Server);
    }
}
