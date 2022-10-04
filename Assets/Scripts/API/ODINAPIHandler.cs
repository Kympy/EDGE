using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ODINAPIHandler : Singleton<ODINAPIHandler>
{
    #region Const Value Use At Getting URL
    private const int DEV_MODE = 1;
    private const int PRODUCT_MODE = 2;
    #endregion

    [Header("Project API Key")]
    [SerializeField] private string ProjectAPIKey = null; // API Key Get From Ocean

    [Header("Base URL for Betting")]
    [SerializeField] private string StagingURL = "https://odin-api-sat.browseosiris.com";
    [SerializeField] private string ProductionURL = "https://odin-api.browseosiris.com";
    [Header("Betting ID")]
    [SerializeField] private string SelectedBettingID = null; // My Betting ID;
    private string GetBettingURL(int type) // Get URL
    {
        if (type == DEV_MODE) return StagingURL; // Return Staging URL When Number 1
        else return ProductionURL; // Else return Production URL
    }
    // Responses
    [SerializeField] private RequestedData.UserProfile ResponseUserProfile;
    [SerializeField] private RequestedData.UserSessionID ResponseUserSessionID;
    [SerializeField] private RequestedData.Res_Settings ResponseBetSettings;
    [SerializeField] private RequestedData.BalanceInfo ResponseBalanceAce;
    [SerializeField] private RequestedData.BalanceInfo ResponseBalanceZera;
    [SerializeField] private RequestedData.BalanceInfo ResponseBalanceDappx;
    [SerializeField] private RequestedData.ResponseBettingPlaceBet ResponseBettingPlaceBet;
    [SerializeField] private RequestedData.ResponseDeclareWinner ResponseDeclareWinner;
    [SerializeField] private RequestedData.ResponseBettingDisconnect ResponseBettingDisconnect;

    // Other Session ID
    private string otherSessionID = "";
    public string OtherSessionID { get { return otherSessionID; } set { otherSessionID = value; } }
    private string otherUserID = "";
    public string OtherUserID { get { return otherUserID; } set { otherUserID = value; } }
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    #region User Profile
    public RequestedData.UserProfile GetUserProfile() // Get User ODIN Profile
    {
        return ResponseUserProfile;
    }
    public IEnumerator ProcessGetUserInfo()
    {
        UnityWebRequest Process = UnityWebRequest.Get("http://localhost:8546/api/getuserprofile");
        yield return Process.SendWebRequest();
        Debug.Log(Process.downloadHandler.text);
        RequestedData.UserProfile? userProfile = JsonUtility.FromJson<RequestedData.UserProfile>(Process.downloadHandler.text);
        if (userProfile != null)
        {
            ResponseUserProfile = (RequestedData.UserProfile)userProfile;
        }
    }
    #endregion
    #region User Session ID
    public RequestedData.UserSessionID GetUserSessionID()
    {
        return ResponseUserSessionID;
    }
    public IEnumerator ProcessGetUserSessionID()
    {
        UnityWebRequest Process = UnityWebRequest.Get("http://localhost:8546/api/getsessionid");
        yield return Process.SendWebRequest();
        Debug.Log(Process.downloadHandler.text);
        //txtInputField.text = Process.downloadHandler.text;
        RequestedData.UserSessionID? userSessionID = JsonUtility.FromJson<RequestedData.UserSessionID>(Process.downloadHandler.text);

        if(userSessionID != null)
        {
            ResponseUserSessionID = (RequestedData.UserSessionID)userSessionID;
        }
    }
    #endregion
    #region Bet Settings
    public RequestedData.Res_Settings GetBetSettings() // Get Betting Info
    {
        return ResponseBetSettings;
    }
    public IEnumerator ProcessGetBetSettings() // Request Betting Settings
    {
        string TargetURL = GetBettingURL(DEV_MODE) + "/v1/betting/settings"; // URL Path

        UnityWebRequest Process = UnityWebRequest.Get(TargetURL);
        Process.SetRequestHeader("api-key", ProjectAPIKey); // Give API Key to header
        yield return Process.SendWebRequest();
        Debug.Log("Bet Settings : " + Process.downloadHandler.text);
        RequestedData.Res_Settings? betSettings = JsonUtility.FromJson<RequestedData.Res_Settings>(Process.downloadHandler.text);
        
        if (betSettings != null)
        {
            ResponseBetSettings = (RequestedData.Res_Settings)betSettings;
            SelectedBettingID = ResponseBetSettings.data.bets[0]._id;
            Debug.Log(SelectedBettingID + " Selected Betting ID");
        }
    }
    #endregion
    #region Check Balance
    public enum COIN_TYPE
    {
        ace,
        zera,
        dappx,
    }
    public RequestedData.BalanceInfo? GetBalance(COIN_TYPE type)
    {
        switch(type)
        {
            case COIN_TYPE.ace:
                {
                    return ResponseBalanceAce;
                }
            case COIN_TYPE.zera:
                {
                    return ResponseBalanceZera;
                }
            case COIN_TYPE.dappx:
                {
                    return ResponseBalanceDappx;
                }
            default:
                {
                    return null;
                }
        }
    }
    public IEnumerator ProcessGetBalance(COIN_TYPE type)
    {
        string URL = GetBettingURL(DEV_MODE) + "/v1/betting/" + type.ToString() + "/balance/" + ResponseUserSessionID.sessionId;
        UnityWebRequest Process = UnityWebRequest.Get(URL);
        Process.SetRequestHeader("api-key", ProjectAPIKey);
        yield return Process.SendWebRequest();
        Debug.Log(Process.downloadHandler.text);
        //txtInputField.text = Process.downloadHandler.text;
        RequestedData.BalanceInfo? balanceInfo = JsonUtility.FromJson<RequestedData.BalanceInfo>(Process.downloadHandler.text);
        
        if(balanceInfo != null)
        {
            switch (type)
            {
                case COIN_TYPE.ace:
                    {
                        ResponseBalanceAce = (RequestedData.BalanceInfo)balanceInfo;
                        break;
                    }
                case COIN_TYPE.zera:
                    {
                        ResponseBalanceZera = (RequestedData.BalanceInfo)balanceInfo;
                        break;
                    }
                case COIN_TYPE.dappx:
                    {
                        ResponseBalanceDappx = (RequestedData.BalanceInfo)balanceInfo;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }        
        }
    }
    #endregion
    #region Bet Coin
    public RequestedData.ResponseBettingPlaceBet GetBet_Coin()
    {
        return ResponseBettingPlaceBet;
    }
    public IEnumerator ProcessBettingCoin(COIN_TYPE type)
    {
        RequestedData.RequestingBettingPlaceBet request_BPB;
        request_BPB.players_session_id = new string[] { ResponseUserSessionID.sessionId, otherSessionID };
        request_BPB.bet_id = SelectedBettingID;

        string URL = GetBettingURL(DEV_MODE) + "/v1/betting/" + type.ToString() + "/place-bet";

        string reqJsonData = JsonUtility.ToJson(request_BPB);
        Debug.Log(reqJsonData);

        UnityWebRequest Process = UnityWebRequest.Post(URL, reqJsonData);
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(reqJsonData);
        Process.uploadHandler = new UploadHandlerRaw(buffer);
        Process.SetRequestHeader("api-key", ProjectAPIKey);
        Process.SetRequestHeader("Content-Type", "application/json");
        yield return Process.SendWebRequest();

        Debug.Log(Process.downloadHandler.text);
        //txtInputField.text = Process.downloadHandler.text;
        RequestedData.ResponseBettingPlaceBet? response_BPB = JsonUtility.FromJson<RequestedData.ResponseBettingPlaceBet>(Process.downloadHandler.text);

        if (response_BPB != null)
        {
            ResponseBettingPlaceBet = (RequestedData.ResponseBettingPlaceBet)response_BPB;
        }
    }
    #endregion
    #region Won_Coin
    public enum Winner
    {
        Me,
        Other,
    }
    public RequestedData.ResponseDeclareWinner GetDeclare_Winner()
    {
        return ResponseDeclareWinner;
    }
    public IEnumerator ProcessRequestDeclareWinner(COIN_TYPE type, Winner winner)
    {
        RequestedData.RequestDeclareWinner request_DeclareWinner;
        request_DeclareWinner.betting_id = ResponseBettingPlaceBet.data.betting_id;
        switch(winner)
        {
            case Winner.Me:
                {
                    request_DeclareWinner.winner_player_id = ResponseUserProfile.userProfile._id;
                    break;
                }
            case Winner.Other:
                {
                    request_DeclareWinner.winner_player_id = otherUserID;
                    break;
                }
            default: { Debug.Log("Winner parameter Error"); request_DeclareWinner.winner_player_id = ""; break; }
        }

        request_DeclareWinner.match_details = null;

        if(request_DeclareWinner.winner_player_id == "")
        {

        }

        string URL = GetBettingURL(DEV_MODE) + "/v1/betting/" + type.ToString() + "/declare-winner";
        string reqJsonData = JsonUtility.ToJson(request_DeclareWinner);
        Debug.Log(reqJsonData);

        UnityWebRequest Process = UnityWebRequest.Post(URL, reqJsonData);
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(reqJsonData);
        Process.uploadHandler = new UploadHandlerRaw(buffer);
        Process.SetRequestHeader("api-key", ProjectAPIKey);
        Process.SetRequestHeader("Content-Type", "application/json");
        yield return Process.SendWebRequest();

        Debug.Log(Process.downloadHandler.text);
        //txtInputField.text = Process.downloadHandler.text;
        RequestedData.ResponseDeclareWinner? response_DeclareWinner = JsonUtility.FromJson<RequestedData.ResponseDeclareWinner>(Process.downloadHandler.text);

        if(response_DeclareWinner != null)
        {
            ResponseDeclareWinner = (RequestedData.ResponseDeclareWinner)response_DeclareWinner;
        }
    }
    #endregion
    #region Disconnect
    public void DisconnectBetting(COIN_TYPE type)
    {
        StartCoroutine(ProcessDisconnectBetting(type));
    }
    private IEnumerator ProcessDisconnectBetting(COIN_TYPE type)
    {
        RequestedData.RequestBettingDisconnect request_Disconnect;
        request_Disconnect.betting_id = ResponseBettingPlaceBet.data.betting_id;

        string URL = GetBettingURL(DEV_MODE) + "/v1/betting/" + type.ToString() + "/disconnect";

        string reqJsonData = JsonUtility.ToJson(request_Disconnect);
        Debug.Log(reqJsonData);

        UnityWebRequest Process = UnityWebRequest.Post(URL, reqJsonData);
        byte[] buff = System.Text.Encoding.UTF8.GetBytes(reqJsonData);
        Process.uploadHandler = new UploadHandlerRaw(buff);
        Process.SetRequestHeader("api-key", ProjectAPIKey);
        Process.SetRequestHeader("Content-Type", "application/json");
        yield return Process.SendWebRequest();

        Debug.Log(Process.downloadHandler.text);
        //txtInputField.text = Process.downloadHandler.text;
        RequestedData.ResponseBettingDisconnect? response_Disconnect = JsonUtility.FromJson<RequestedData.ResponseBettingDisconnect>(Process.downloadHandler.text);
        
        if(response_Disconnect != null)
        {
            ResponseBettingDisconnect = (RequestedData.ResponseBettingDisconnect)response_Disconnect;
        }
    }
    #endregion
}
