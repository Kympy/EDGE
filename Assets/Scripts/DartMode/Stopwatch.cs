using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stopwatch : MonoBehaviour
{
    public Text text_Timer;
    float time_start;
    float time_current;
    float time_Max = 10f;
    bool isEnded;

    // Start is called before the first frame update
    void Start()
    {
        Reset_Timer();
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnded)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("스톱워치 시작");
            Check_Timer();
        }
    }

    void Check_Timer()
    {
        time_current = Time.time - time_start;
        if (time_current < time_Max)
        {
            text_Timer.text = "{time_current:N2}";
        }

        else if (!isEnded)
        {
            End_Timer();
        }
    }

    void End_Timer()
    {
        time_current = time_Max;
        text_Timer.text = $"{time_current:N2}";
        isEnded = true;
    }

    void Reset_Timer()
    {
        time_start = Time.time;
        time_current = 0;
        text_Timer.text = $"{time_current:N2}";
        isEnded = false;
    }
}
