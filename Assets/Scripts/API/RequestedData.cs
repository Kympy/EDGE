using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RequestedData
{
    #region User Info
    [Serializable]
    public struct Profile
    {
        // User Profile
        public string referral_by;
        public string referral_code;
        public string username;
        public string email_id;
        public string public_address;
        public string _id;
        public string upline;
    }
    [Serializable]
    public struct UserProfile
    {
        // Response Result
        public string Status;
        public int StatusCode;
        public string Message;
        public Profile userProfile;
    }
    [Serializable]
    public struct UserSessionID
    {
        // Response Result
        public string Status;
        public int StatusCode;
        public string Message;
        // User Session ID
        public string sessionId;
    }
    #endregion
    #region CheckBalance
    [Serializable]
    public struct Balance
    {
        public string balance;
    }
    [Serializable]
    public struct BalanceInfo
    {
        // Response Result
        public string message;
        // Balance
        public Balance data;
    }
    #endregion
    #region Bettings
    [Serializable]
    public struct Res_Settings
    {
        public string message;
        public Settings data;
    }
    [Serializable]
    public struct Settings
    {
        public string _id;
        public string game_id;
        public bool betting;
        public bool maintenance;
        public string createdAt;
        public string updatedAt;
        public int __v;

        public BetInfo[] bets;
    }
    [Serializable]
    public struct BetInfo
    {
        public string _id;
        public string game_id;
        public int amount;
        public int platform_fee;
        public int developer_fee;
        public int win_reward;
        public int win_amount;
        public string createdAt;
        public string updatedAt;
        public int __v;
    }
    [Serializable]
    public struct RequestingBettingPlaceBet
    {
        public string[] Players_Session_ID;
        public string Bet_ID;
    }

    [Serializable]
    public struct ResponseBettingPlaceBet
    {
        // Response Result
        public string Message;
        // Data
        [Serializable]
        public struct Data
        {
            public string betting_id;
        }
        public Data data;
    }
    [Serializable]
    public struct RequestDeclareWinner
    {
        public string Betting_ID;
        public string Winner_ID;
        public object Match_Details;
    }
    [Serializable]
    public struct ResponseDeclareWinner
    {
        // Response Result
        public string Message;
        // Amount of Winner Earn
        [Serializable]
        public struct Data
        {
            public int amount_win;
        }
        public Data data;
    }
    [Serializable]
    public struct RequestBettingDisconnect
    {
        public string Betting_ID;
    }
    [Serializable]
    public struct ResponseBettingDisconnect
    {
        // Response Result
        public string Message;
        [Serializable]
        public struct Data
        {

        }
        public Data data;
    }
    #endregion
}
