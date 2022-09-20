using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class UIManager : MonoBehaviourPun
{
    //private TextMeshProUGUI Player_1_Name = null;
    //private TextMeshProUGUI Player_2_Name = null;
    [SerializeField] private TextMeshProUGUI EnemyPlayerName = null;

    private TextMeshProUGUI MyHP = null;

    private SniperControl MyPlayer = null;
    [SerializeField] SniperGameManager MyGameManager = null;

    private void Start()
    {
        EnemyPlayerName.text = "";
    }
    public void InitUserHP()
    {
        MyPlayer = FindObjectOfType<SniperControl>();
        MyHP = GameObject.Find("HPInfo").GetComponent<TextMeshProUGUI>();
        MyHP.text = MyPlayer.CurrentHP + " / " + MyPlayer.Max_HP;
    }
    public void UpdateEnemyName(string nickname)
    {
        EnemyPlayerName.text = nickname;
    }
    public void SetNickNamePosition(string name, Vector3 position)
    {
        EnemyPlayerName.gameObject.SetActive(true);
        Debug.Log("Position : " + position);
        EnemyPlayerName.text = name;
        EnemyPlayerName.rectTransform.position = new Vector3(position.x, position.y);
    }
    public void HideNickName()
    {
        EnemyPlayerName.gameObject.SetActive(false);
    }
}
