using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class MainLobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Room Info")] // Rooms
    [SerializeField] private RectTransform[] RoomPosition = new RectTransform[6];
    [SerializeField] private bool[] IsRoomExist = new bool[6]; // Is there a room
    [SerializeField] private Button CreateButton = null;
    [Header("Page Info")] // Pages
    [SerializeField] private Button NextPage = null;
    [SerializeField] private Button PreviousPage = null;
    [SerializeField] private TextMeshProUGUI PageInfo = null;

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //base.OnRoomListUpdate(roomList);
    }
}
