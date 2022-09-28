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
    public struct BetSettings
    {
        // Response Result
        public string Message;
        public string ID;
        public string Game_ID;
        public bool Betting;
        public bool Maintenance;
        public string CreatedAt;
        public string UpdatedAt;
        public int __V;
    }
    [Serializable]
    public struct BetInfo
    {
        public string ID;
        public string Game_ID;
        public int Amount;
        public int Platform_Fee;
        public int Developer_Fee;
        public int Win_Reward;
        public int Win_Amount;
        public string CreatedAt;
        public string UpdatedAt;
        public int __V;
    }
    [Serializable]
    public struct BetData
    {
        public BetSettings betSettings;
        public BetInfo[] betInfo;
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
        public string Bet_ID;
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
        public int Amount_Won;
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
    }
    #endregion
}
