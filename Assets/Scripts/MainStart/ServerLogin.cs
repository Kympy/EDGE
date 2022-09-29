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
    [SerializeField] private RectTransform UserLoginPanel = null;
    [SerializeField] private TextMeshProUGUI UserName = null;

    [SerializeField] private Canvas Warning = null;
    [SerializeField] private Button OKButton = null;
    [SerializeField] private TextMeshProUGUI WarningMsg = null;

    [SerializeField] private TextMeshProUGUI PressAnyKey = null;

    private bool FinishLogin = false;
    private const string Press = "PRESS ANY KEY";
    private const string Load = "CONNECTING";
    private void Awake()
    {
        PressAnyKey.text = Load;
        StartCoroutine(GetMyInfo());

        PhotonNetwork.GameVersion = "0.1";
        PhotonNetwork.ConnectUsingSettings(); // Applicate Connection to Master Server
        OKButton.onClick.AddListener(delegate
        {
            Warning.gameObject.SetActive(false);
        });
        WarningMsg.text = "";
        Warning.gameObject.SetActive(false);
    }
    private void Update()
    {
        PressAnyKey.alpha = Mathf.PingPong(Time.time, 1f);

        if(FinishLogin && Input.anyKeyDown)
        {
            LoginStart();
        }
    }
    private IEnumerator GetMyInfo()
    {
        yield return OceanAPIHandler.Instance.ProcessGetUserInfo();
        yield return OceanAPIHandler.Instance.ProcessGetUserSessionID();
        RequestedData.UserProfile userProfile = OceanAPIHandler.Instance.GetUserProfile();
        UserName.text = "Welcome, " + userProfile.userProfile.username + "!";
        PhotonNetwork.NickName = userProfile.userProfile.username; // Set Nickname
        while (true)
        {
            if(PhotonNetwork.IsConnectedAndReady)
            {
                UserLoginPanel.anchoredPosition += new Vector2(0f, -3f);
                if(UserLoginPanel.anchoredPosition.y < -99f)
                {
                    UserLoginPanel.anchoredPosition = new Vector2(0f, -99f);
                    StartCoroutine(Hide());
                    PressAnyKey.text = Press;
                    FinishLogin = true;
                    yield break;
                }
            }
            yield return null;
        }
    }
    private IEnumerator Hide()
    {
        yield return new WaitForSeconds(3f);
        while(true)
        {
            if(Input.anyKeyDown)
            {
                yield break;
            }
            UserLoginPanel.anchoredPosition += new Vector2(0f, 3f);
            if (UserLoginPanel.anchoredPosition.y > 0f)
            {
                UserLoginPanel.anchoredPosition = new Vector2(0f, 0f);
                yield break;
            }
            yield return null;
        }
    }
    private void LoginStart()
    {
        StartCoroutine(JoinLobbyCo());
    }
    private void PopUpMsg(string Message)
    {
        WarningMsg.text = Message;
        Warning.gameObject.SetActive(true);
    }
    private IEnumerator JoinLobbyCo()
    {
        PopUpMsg("Connecting.");
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
