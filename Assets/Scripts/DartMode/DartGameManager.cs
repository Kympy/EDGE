using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class DartGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject PlayerSpawnPosition;
    [SerializeField] TextMeshPro Board1;
    [SerializeField] TextMeshPro Board2;
    [SerializeField] TextMeshProUGUI EndGameText;

    public int GameEnd = 0;
    

    // Start is called before the first frame update
    private void Awake()
    {
        //if (photonView.IsMine == false) return;
        PhotonNetwork.Instantiate("DartMode/Player", PlayerSpawnPosition.transform.position, PlayerSpawnPosition.transform.rotation);

        EndGameText.text = " ";

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(ODINAPIHandler.Instance.ProcessBettingCoin(ODINAPIHandler.COIN_TYPE.zera));
        }
    }


    [PunRPC]
    void EndGame()
    {
        GameEnd++;
        if (GameEnd == 2)
        {
            float _1pScore = float.Parse(Board1.text);
            float _2pScore = float.Parse(Board2.text);

            PhotonNetwork.AutomaticallySyncScene = true;

            if (_1pScore > _2pScore)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    // Win
                    EndGameText.text = "You Win!!";
                    StartCoroutine(ODINAPIHandler.Instance.ProcessRequestDeclareWinner(ODINAPIHandler.COIN_TYPE.zera, ODINAPIHandler.Winner.Me));
                }

                else
                {
                    // lose
                    EndGameText.text = "You Lose!!";

                }
            }
            else if(_1pScore < _2pScore)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    // Lose
                    EndGameText.text = "You Lose!!";
                    StartCoroutine(ODINAPIHandler.Instance.ProcessRequestDeclareWinner(ODINAPIHandler.COIN_TYPE.zera, ODINAPIHandler.Winner.Other));
                }

                else
                {
                    // win
                    EndGameText.text = "You Win!!";
                }
            }
            else
            {
                // draw
                EndGameText.text = "Draw Game!!";
                if (PhotonNetwork.IsMasterClient)
                {
                    StartCoroutine("ReStartGame");
                    return;
                }
            }

            if(PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(LoadRoom());
            }
        }
    }
    private IEnumerator LoadRoom()
    {
        yield return new WaitForSecondsRealtime(5f);
        PhotonNetwork.LoadLevel("RoomScene");
    }

    private IEnumerator ReStartGame()
    {
        yield return new WaitForSecondsRealtime(5f);
        PhotonNetwork.LoadLevel("LoadingScene");
    }
}
