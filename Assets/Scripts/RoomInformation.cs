using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomInformation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI RoomName;
    [SerializeField] private TextMeshProUGUI BetAmount;

    public void InitRoom(string Name, string Amount)
    {
        RoomName.text = Name;
        BetAmount.text = "Bet : " + Amount + " $";
    }
}
