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

    [SerializeField] private ChatManager _ChatManager = null;

    private int CurrentMode = 0;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
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
        //InitRoom();
        //ShowUser();
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
    }
    private IEnumerator Start()
    {
        yield return InitRoom();
        while(true)
        {
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnectedAndReady)
            {
                StartCoroutine(CountDown());
                yield break;
            }
            yield return null;
        }
    }
    public IEnumerator InitRoom()
    {
        while(true)
        {
            if(PhotonNetwork.IsConnectedAndReady)
            {
                break;
            }
            yield return null;
        }

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

        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Mode", out object mode);
        if (mode != null)
        {
            RoomTitle.text += " - " + mode.ToString();
            if(mode.ToString() == "Sniper Mode")
            {
                CurrentMode = 1;
            }
            else if(mode.ToString() == "Pistol Mode")
            {
                CurrentMode = 2;
            }
            else if(mode.ToString() == "Dart Mode")
            {
                CurrentMode = 3;
            }
        }
        ShowUser();
        yield return null;
    }
    public void ShowUser()
    {
        Debug.Log("Created");
        GameObject userbox = PhotonNetwork.Instantiate("SniperMode/Rooms/UserBox", Vector3.one, Quaternion.identity);
        if (photonView.IsMine)
        {
            userbox.GetPhotonView().RPC("InitUserUI", RpcTarget.AllBuffered, PhotonNetwork.NickName, "20.0", "12", 1, User1Pos.position);
        }
        else
        {
            userbox.GetPhotonView().RPC("InitUserUI", RpcTarget.AllBuffered, PhotonNetwork.NickName, "40.0", "232", 2, User2Pos.position);
        }
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
        Debug.Log("Edit");
        ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;
        hash.Remove("Bet");
        hash.Remove("RoomName");
        hash.Remove("Password");
        hash.Add("Bet", BetInput.text);
        hash.Add("RoomName", NameInput.text);
        hash.Add("Password", PassInput.text);

        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        InitRoom();
        ToggleEditUI(false);
    }
    private void ExitRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(); // Rejoin Lobby when leave room
        PhotonNetwork.LoadLevel(1); // Go to lobby scene
    }
    public IEnumerator CountDown()
    {
        int time = 3;
        float timer = 0f;
        while(true)
        {
            if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                if (User1Pos.transform.childCount > 0 && User2Pos.transform.childCount > 0)
                {
                    if (User1Pos.GetChild(0).GetComponent<UserBox>().GetReadyText.activeSelf &&
                        User2Pos.GetChild(0).GetComponent<UserBox>().GetReadyText.activeSelf)
                    {
                        timer += Time.deltaTime;
                        if(timer > 1f)
                        {
                            if (time <= 0)
                            {
                                if(CurrentMode != 0)
                                {
                                    if(CurrentMode == 1)
                                    {
                                        PhotonNetwork.LoadLevel("SniperScene");
                                    }
                                    else if(CurrentMode == 2)
                                    {
                                        PhotonNetwork.LoadLevel(4);
                                    }
                                    else if(CurrentMode == 3)
                                    {
                                        PhotonNetwork.LoadLevel(5);
                                    }
                                    else
                                    {
                                        _ChatManager.AddChatLine("System", "Load Game Mode Failed");
                                    }
                                    yield break;
                                }
                            }
                            photonView.RPC("ChatLineMsg", RpcTarget.All, "System", "Enter the game in a few moments..." + time);
                            time--;
                            timer = 0f;
                        }
                    }
                    else
                    {
                        time = 3;
                        timer = 0f;
                    }
                }
            }
            yield return null;
        }
    }
    [PunRPC]
    public void ChatLineMsg(string name, string msg)
    {
        _ChatManager.AddChatLine(name, msg);
    }
#if UNITY_EDITOR
    private void OnGUI()
    {
        GUI.Label(new Rect(20f, 50f, 200f, 20f), "Client State : " + PhotonNetwork.NetworkClientState.ToString());
        GUI.Label(new Rect(20f, 90f, 200f, 20f), "Server State : " + PhotonNetwork.Server);
    }
#endif
}
