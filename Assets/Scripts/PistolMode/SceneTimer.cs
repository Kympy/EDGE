using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class SceneTimer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI lobbyTimer;
    float timeNum = 10.5f;


    // Update is called once per frame
    void FixedUpdate()
    {
        LobbyTimer();
        lobbyTimer.text = ((int)timeNum).ToString();
    }

    void LobbyTimer()
    {
        if (timeNum <= 10.5 && timeNum >= 0)
        {
            timeNum -= Time.deltaTime;
        }
    }

}
