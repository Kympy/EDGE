using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
//using System.Runtime.InteropServices;
public class ServerLogin : MonoBehaviourPunCallbacks
{
    //[DllImport("user32.dll")]
    //private static extern int SetCursorPos(int X, int Y);
    [SerializeField] private RectTransform UserLoginPanel = null; // Top Login UI Panel
    [SerializeField] private TextMeshProUGUI UserName = null; // My Nickname from ODIN

    [SerializeField] private Canvas Warning = null; // Pop up box
    [SerializeField] private TextMeshProUGUI WarningMsg = null; // Pop up box text

    [SerializeField] private TextMeshProUGUI PressAnyKey = null; // Bottom Text UI

    private bool FinishLogin = false; // Is Finish Login And Get Data?
    // Const string to input bottom text UI
    private const string Press = "PRESS ANY KEY";
    private const string Load = "CONNECTING";
    private void Awake()
    {
        PressAnyKey.text = Load; // "CONNECTING"

        StartCoroutine(GetMyInfo()); // Request Get User Information

        PhotonNetwork.GameVersion = "0.1"; // Set Game Version
        PhotonNetwork.ConnectUsingSettings(); // Applicate Connection to Master Server
        WarningMsg.text = ""; // Init
        Warning.gameObject.SetActive(false); // Pop Up Box disable
    }
    private void Update()
    {
        PressAnyKey.alpha = Mathf.PingPong(Time.time, 1f); // Ping Pong Text Blink (Bottom Text)

        if(FinishLogin && Input.anyKeyDown) // When finished getting user data and input any key
        {
            LoginStart(); // Start load a lobby scene.
        }
    }
    #region Get User Info And Display On UI
    private IEnumerator GetMyInfo() // Request User Data And Set
    {
        yield return ODINAPIHandler.Instance.ProcessGetUserInfo(); // Request and wait for getting user info data
        yield return ODINAPIHandler.Instance.ProcessGetUserSessionID(); // Request and wait for getting user session id
        RequestedData.UserProfile userProfile = ODINAPIHandler.Instance.GetUserProfile(); // Get saved user profile data
        UserName.text = "Welcome, " + userProfile.userProfile.username + "!"; // Set UI
        PhotonNetwork.NickName = userProfile.userProfile.username; // Set photon nickname
        
        while(true) // loop
        {
            if(PhotonNetwork.IsConnectedAndReady) // Get Ready
            {
                UserLoginPanel.anchoredPosition += new Vector2(0f, -3f); // Panel down
                if(UserLoginPanel.anchoredPosition.y < -99f) // If Max
                {
                    UserLoginPanel.anchoredPosition = new Vector2(0f, -99f); // Stop
                    StartCoroutine(Hide()); // Start hide coroutine
                    PressAnyKey.text = Press; // Change bottom ui text
                    FinishLogin = true; // Allow Scene change
                    yield break;
                }
            }
            yield return null;
        }
    }
    private IEnumerator Hide() // Hide user info panel
    {
        yield return new WaitForSeconds(3f); // Freeze 3 seconds
        while(true)
        {
            if(Input.anyKeyDown) // If input any key, load next level and stop coroutine
            {
                yield break;
            }
            UserLoginPanel.anchoredPosition += new Vector2(0f, 3f); // Panel Up
            if (UserLoginPanel.anchoredPosition.y > 0f) // If max,
            {
                UserLoginPanel.anchoredPosition = new Vector2(0f, 0f); // Stop
                yield break;
            }
            yield return null;
        }
    }
    #endregion
    #region Load Lobby
    private void LoginStart() // Start load lobby scene
    {
        StartCoroutine(JoinLobbyCo()); // Start coroutine
    }
    private IEnumerator JoinLobbyCo() // Wait and load
    {
        PopUpMsg("Connecting."); // Pop up box message
        yield return new WaitForSecondsRealtime(0.5f); // Wait (Because of the dynamic effect)
        WarningMsg.text += "."; // Add Dot
        yield return new WaitForSecondsRealtime(0.5f);
        WarningMsg.text += "."; // Add Dot
        yield return new WaitForSecondsRealtime(0.5f);
        PhotonNetwork.JoinLobby(); // Load Lobby After 1.5 seconds
        while (true)
        {
            if(PhotonNetwork.InLobby) // If in lobby,
            {
                yield break; // Stop this coroutine.
            }
            yield return null;
        }
    }
    public override void OnJoinedLobby() // When success to join lobby,
    {
        PhotonNetwork.LoadLevel(1); // Change scene to lobby (level 1)
    }
    #endregion
    private void PopUpMsg(string Message) // Pop up box message can receive msg
    {
        WarningMsg.text = Message; // Input text
        Warning.gameObject.SetActive(true); // Show
    }
}
