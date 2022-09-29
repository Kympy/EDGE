using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class JsonDataManager // Save And Load Data System
{
    // Save Path
    private static readonly string UserSettingPath = Application.persistentDataPath + "/UserSettings/";

    // Data Struct
    public struct UserSetting
    {
        // Mouse Sensitivitiy
        public float Mouse_X_Sensitivity;
        public float Mouse_Y_Sensitivity;
        public float MouseScrollWheel_Sensitivity;
        public bool MouseVerticalInverse;
        public bool FullScreenMode; // Is full screen?
    }
    
    public static void SaveData(UserSetting SaveUserSet) // Save User Data and convert to json file
    {
        if(Directory.Exists(UserSettingPath) == false) // Check directory exist
        {
            Directory.CreateDirectory(UserSettingPath); // Create directory
        }
        string JsonData = JsonUtility.ToJson(SaveUserSet); // Convert to json from struct data
        File.WriteAllText(UserSettingPath + "UserSetting.json", JsonData); // Write file
        Debug.Log("File Write Success : " + UserSettingPath);
    }
    public static UserSetting? LoadData(string fileName) // fileName Rule => "name.json"
    {
        if(File.Exists(UserSettingPath + fileName) == false) // Check file exist
        {
            return null; // Exception Error
        }
        string JsonData = File.ReadAllText(UserSettingPath + fileName); // Read all text data
        UserSetting userSettingData = JsonUtility.FromJson<UserSetting>(JsonData); // Convert to struct data from jsom
        return userSettingData; // Return data
    }
}
