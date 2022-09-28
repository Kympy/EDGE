using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
//using System.Runtime.InteropServices;
public class ServerLogin : MonoBehaviourPunCallbacks
{
    //[DllImport("user32.dll")]
    //private static extern int SetCursorPos(int X, int Y);

    [SerializeField] private Button LoginButton = null;
    [SerializeField] private TMP_InputField NicknameInput = null;

    [SerializeField] private Canvas Warning = null;
    [SerializeField] private Button OKButton = null;
    [SerializeField] private TextMeshProUGUI WarningMsg = null;

    private void Awake()
    {
        LoginButton.interactable = false;
        StartCoroutine(GetMyInfo());

        PhotonNetwork.GameVersion = "0.1";
        PhotonNetwork.ConnectUsingSettings(); // Applicate Connection to Master Server
        OKButton.onClick.AddListener(delegate
        {
            NicknameInput.text = "";
            Warning.gameObject.SetActive(false);
        });
        WarningMsg.text = "";
        Warning.gameObject.SetActive(false);
        NicknameInput.onEndEdit.AddListener(delegate { LoginStart(); });
        LoginButton.onClick.AddListener(() => LoginStart());
    }
    private IEnumerator GetMyInfo()
    {
        yield return OceanAPIHandler.Instance.ProcessGetUserInfo();
        yield return OceanAPIHandler.Instance.ProcessGetUserSessionID();
        RequestedData.UserProfile userProfile = OceanAPIHandler.Instance.GetUserProfile();
        NicknameInput.text = userProfile.userProfile.username;
        
        while(true)
        {
            if(PhotonNetwork.IsConnectedAndReady)
            {
                LoginButton.interactable = true;
                yield break;
            }
            yield return null;
        }
    }
    private void LoginStart()
    {
        PhotonNetwork.NickName = NicknameInput.text; // Set Nickname
        StartCoroutine(JoinLobbyCo());
    }
    private void NicknameWarningUI(string Message)
    {
        WarningMsg.text = Message;
        Warning.gameObject.SetActive(true);
    }
    private IEnumerator JoinLobbyCo()
    {
        NicknameWarningUI("Connecting.");
        OKButton.gameObject.SetActive(false);
        yield return new WaitForSecondsRealtime(0.5f);
        WarningMsg.text += ".";
        yield return new WaitForSecondsRealtime(0.5f);
        WarningMsg.text += ".";
        yield return new WaitForSecondsRealtime(0.5f);
        PhotonNetwork.JoinLobby();
        while (true)
        {
            if(PhotonNetwork.InLobby)
            {
                yield break;
            }
            yield return null;
        }
    }
    public override void OnJoinedLobby()
    {
        PhotonNetwork.LoadLevel(1);
    }
}
