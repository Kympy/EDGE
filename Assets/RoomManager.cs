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

    [SerializeField] private GameObject userbox = null;
    private bool[] slot = { false, false };
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
        Debug.Log(PhotonNetwork.NickName + " Awake");

        //photonView.RPC("ShowUser", RpcTarget.All);
        ShowUser();
    }
    [PunRPC]
    public void ChangeSlot(int index, bool value)
    {
        slot[index] = value;
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
    public void ShowUser()
    {
        Debug.Log("Created");
        userbox = PhotonNetwork.Instantiate("Rooms/UserBox", Vector3.one, Quaternion.identity);
        if (photonView.IsMine)
        {
            Debug.Log("Master");
            userbox.transform.position = User1Pos.position;
            userbox.transform.SetParent(User1Pos);
            userbox.GetPhotonView().RPC("InitUserUI", RpcTarget.AllBuffered, PhotonNetwork.NickName, "20.0", "12", 1);
            //userbox.GetComponent<UserBox>().InitUserUI(PhotonNetwork.NickName, "20.0", "12", 1);
        }
        else
        {
            Debug.Log("Client");
            userbox.transform.position = User2Pos.position;
            userbox.transform.SetParent(User2Pos);
            userbox.GetPhotonView().RPC("InitUserUI", RpcTarget.AllBuffered, PhotonNetwork.NickName, "40.0", "232", 2);
            //userbox.GetComponent<UserBox>().InitUserUI(PhotonNetwork.NickName, "60.0", "357", 2);
        }
    }
    private void OnGUI()
    {
        GUI.Label(new Rect(20f, 50f, 200f, 20f), "Client State : " + PhotonNetwork.NetworkClientState.ToString());
        //GUI.Label(new Rect(20f, 70f, 200f, 20f), "RoomName : " + PhotonNetwork.CurrentRoom.Name);
        GUI.Label(new Rect(20f, 90f, 200f, 20f), "Server State : " + PhotonNetwork.Server);
    }
}
