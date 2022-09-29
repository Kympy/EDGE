using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BullCount : MonoBehaviour
{
    [SerializeField]
    GameObject[] curBullet = new GameObject[6];

    public void countBullet(int cBullet)
    {
        for (int i = 0; i < cBullet; i++)
        {
            curBullet[i].SetActive(false);
        }
    }
}
