using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
public class UserBox : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI UserName = null;
    [SerializeField] private TextMeshProUGUI UserInfo = null;
    [SerializeField] private GameObject ReadyText = null;
    [SerializeField] private Button ReadyButton = null;

    private void Awake()
    {
        ReadyText.SetActive(false);
        ReadyButton.onClick.AddListener(() => ClickReady());
        photonView.RPC("DisableRemoteButton", RpcTarget.OthersBuffered);
    }
    public void ClickReady()
    {
        photonView.RPC("PlayerReady", RpcTarget.AllBuffered, !ReadyText.activeSelf);
    }
    [PunRPC]
    public void DisableRemoteButton()
    {
        ReadyButton.gameObject.SetActive(false);
    }
    [PunRPC]
    public void PlayerReady(bool isReady)
    {
        ReadyText.SetActive(isReady);
    }
    [PunRPC]
    public void InitUserUI(string name, string rating, string games, int ID, Vector3 position)
    {
        UserName.text = name;
        this.transform.position = position;
        
        if (ID == 1)
        {
            UserInfo.alignment = TextAlignmentOptions.MidlineLeft;
            this.transform.SetParent(GameObject.Find("User1Pos").transform);
        }
        else
        {
            UserInfo.alignment = TextAlignmentOptions.MidlineRight;
            this.transform.SetParent(GameObject.Find("User2Pos").transform);
        }
        UserInfo.text = "Rating : " + rating + "%\n" + "Games : " + games;
    }
}
