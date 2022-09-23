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
    // Slang
    public List<string> ForbiddenWords = new List<string>();

    [SerializeField] private Canvas Warning = null;
    [SerializeField] private Button OKButton = null;
    [SerializeField] private TextMeshProUGUI WarningMsg = null;

    private void Awake()
    {
        ForbiddenWords.Add("Fuck");
        ForbiddenWords.Add("fuck");
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
    private void LoginStart()
    {
        if (IsSafeNickname() == false) return;

        PhotonNetwork.NickName = NicknameInput.text; // Set Nickname
        StartCoroutine(JoinLobbyCo());
        //PhotonNetwork.JoinOrCreateRoom("TestRoom", new RoomOptions { MaxPlayers = 2 }, null); // Check Room and Join or Create
    }
    private bool IsSafeNickname()
    {
        if (NicknameInput.text == "") // Not satisfying Name Rule
        {
            NicknameWarningUI("Please Input Your Nickname.");
            return false;
        }
        if(NicknameInput.text.Length < 2 || NicknameInput.text.Length > 12)
        {
            NicknameWarningUI("Nickname length must in range\nbetween 2 to 12.");
            return false;
        }
        foreach(string text in ForbiddenWords) // Has forbidden word?
        {
            if(NicknameInput.text.Contains(text))
            {
                NicknameWarningUI("\"" + text + "\"" + " is forbidden word.");
                return false;
            }
        }
        return true;
    }
    private void NicknameWarningUI(string Message)
    {
        WarningMsg.text = Message;
        Warning.gameObject.SetActive(true);
    }
    private IEnumerator JoinLobbyCo()
    {
        NicknameWarningUI(PhotonNetwork.NetworkClientState.ToString());
        yield return new WaitForSecondsRealtime(1f);
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        while (true)
        {
            NicknameWarningUI(PhotonNetwork.NetworkClientState.ToString());
            OKButton.gameObject.SetActive(false);
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
