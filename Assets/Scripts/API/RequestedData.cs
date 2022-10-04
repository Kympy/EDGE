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
        [Serializable]
        public struct Data
        {
            public Settings settings;
            public BetInfo[] bets;
        }
        public Data data;
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
        public string[] players_session_id;
        public string bet_id;
    }

    [Serializable]
    public struct ResponseBettingPlaceBet
    {
        // Response Result
        public string message;
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
        public string betting_id;
        public string winner_player_id;
        public object match_details;
    }
    [Serializable]
    public struct ResponseDeclareWinner
    {
        // Response Result
        public string message;
        // Amount of Winner Earn
        [Serializable]
        public struct Data
        {
            public int amount_won;
        }
        public Data data;
    }
    [Serializable]
    public struct RequestBettingDisconnect
    {
        public string betting_id;
    }
    [Serializable]
    public struct ResponseBettingDisconnect
    {
        // Response Result
        public string message;
        [Serializable]
        public struct Data
        {

        }
        public Data data;
    }
    #endregion
}
