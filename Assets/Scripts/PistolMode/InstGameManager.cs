using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InstGameManager : MonoBehaviourPun
{

    private void Awake()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();

        if (gameManager != null)
        {
            Debug.Log("GameManager Already Exist!!");

            return;
        }
        else
        {
            Debug.Log("There's no GameManager. Create new");

            GameObject GameManagerViewID = PhotonNetwork.Instantiate("PistolMode/GameManager", Vector3.zero, Quaternion.identity);

/*
            GameObject gameManagerObj = new GameObject(typeof(GameManager).ToString());
            gameManagerObj.AddComponent<GameManager>();
            gameManagerObj.AddComponent<PhotonView>();
*/
            
        }
    }
}
