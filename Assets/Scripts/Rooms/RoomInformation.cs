using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class RoomInformation : MonoBehaviourPun
{
    private string RoomID = "";
    private string Password = "";
    [SerializeField] private TextMeshProUGUI RoomName;
    [SerializeField] private TextMeshProUGUI BetAmount;
    [SerializeField] private GameObject LockIcon;
    [SerializeField] private GameObject PassPanel;
    [SerializeField] private TMP_InputField PassInput;
    [SerializeField] private Button Cancel;
    private void Awake()
    {
        PassInput.onEndEdit.AddListener(delegate { CheckPassword(PassInput.text); });
        Cancel.onClick.AddListener(delegate { PassPanel.SetActive(false); });
        PassPanel.SetActive(false);
        LockIcon.SetActive(false);
        this.GetComponent<Button>().onClick.AddListener(() => JoinRoom());
    }

    public void InitRoom(string Name, string Amount, bool locked, string ID, string password)
    {
        Debug.Log("RoomInit");
        RoomName.text = Name;
        BetAmount.text = "Bet : " + Amount + " $";
        LockIcon.SetActive(locked);
        RoomID = ID;
        Password = password;
    }
    public void JoinRoom()
    {
        if(LockIcon.activeSelf)
        {
            PassPanel.SetActive(true);
        }
        else
        {
            PhotonNetwork.JoinRoom(RoomID);
        }
    }
    public void CheckPassword(string text)
    {
        if (Password == text)
        {
            PhotonNetwork.JoinRoom(RoomID);
        }
        else PassInput.text = "";
    }
    /*
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(2);
    }
    */
}
