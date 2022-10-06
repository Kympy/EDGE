using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
// 메쉬프로 //

public class ScoreText : MonoBehaviour
{
    [SerializeField] int TotalScore = 0;
    public int getScore { get { return TotalScore; } set { TotalScore = value; } }
    TextMeshPro TextPosition;

    public void AddScore()
    {
        gameObject.GetComponent<PhotonView>().RPC("UpdateScore", RpcTarget.All, TotalScore);
    }

    // Start is called before the first frame update

    private void Awake()
    {
        TextPosition = this.gameObject.GetComponent<TextMeshPro>();
        TextPosition.text = "0";
    }

    [PunRPC]
    public void UpdateScore(int TotalScore)
    {
        TextPosition.text = TotalScore.ToString();
    }

    

}
