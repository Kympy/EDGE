using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class UIManager : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI EnemyPlayerName = null;

    [SerializeField] private GameObject TabPanel = null;
    [SerializeField] private TextMeshProUGUI User1Name = null;
    [SerializeField] private TextMeshProUGUI User2Name = null;
    [System.Serializable]
    public struct Indicator
    {
        public Image Head;
        public Image Body;
        public Image ArmL;
        public Image ArmR;
        public Image LegL;
        public Image LegR;
    }

    [SerializeField] private Indicator MyIndicator;
    [SerializeField] private Indicator EnemyIndicator;

    [SerializeField] private TextMeshProUGUI MyHP = null;

    private SniperControl MyPlayer = null;
    [SerializeField] SniperGameManager MyGameManager = null;

    private void Start()
    {
        EnemyPlayerName.text = "";
        User1Name.text = PhotonNetwork.NickName;
        User2Name.text = "";
        TabPanel.SetActive(false);
    }
    private void Update()
    {
        if(Input.GetKey(KeyCode.Tab))
        {
            TabPanel.SetActive(true);
        }
        else if(Input.GetKeyUp(KeyCode.Tab))
        {
            TabPanel.SetActive(false);
        }
    }
    public void InitUserHP()
    {
        MyPlayer = MyGameManager.PlayerList[0].GetComponent<SniperControl>();
        MyHP.text = MyPlayer.CurrentHP + " / " + MyPlayer.Max_HP;
    }
    public void UpdateHP()
    {
        MyHP.text = MyPlayer.CurrentHP + " / " + MyPlayer.Max_HP;
    }
    public void UpdateEnemyName(string nickname)
    {
        EnemyPlayerName.text = nickname;
        User2Name.text = nickname;
    }
    public void SetNickNamePosition(string name, Vector3 position)
    {
        EnemyPlayerName.gameObject.SetActive(true);
        //Debug.Log("Position : " + position);
        EnemyPlayerName.text = name;
        User2Name.text = name;
        EnemyPlayerName.rectTransform.position = new Vector3(position.x, position.y);
    }
    public void HideNickName()
    {
        EnemyPlayerName.gameObject.SetActive(false);
    }
    public void UpdateIndicator(string parts, int playerIndex)
    { 
        switch(playerIndex)
        {
            case 1:
                {
                    SetIndicator(MyIndicator, parts);
                    break;
                }
            case 2:
                {
                    SetIndicator(EnemyIndicator, parts);
                    break;
                }
            default:
                {
                    Debug.Log("Player Index Error : Change Parts Red() ");
                    break;
                }
        }
    }
    public void SetIndicator(Indicator someone, string parts)
    {
        if(parts == "Head")
        {
            someone.Head.sprite = MyGameManager.prefabData.HeadRed;
            someone.Head.color = Color.white;
        }
        else if(parts == "Body")
        {
            someone.Body.sprite = MyGameManager.prefabData.BodyRed;
            someone.Body.color = Color.white;
        }
        else if(parts == "ArmL")
        {
            someone.ArmL.sprite = MyGameManager.prefabData.ArmL;
            someone.ArmL.color = Color.white;
        }
        else if(parts == "ArmR")
        {
            someone.ArmR.sprite = MyGameManager.prefabData.ArmR;
            someone.ArmR.color = Color.white;
        }
        else if(parts == "LegL")
        {
            someone.LegL.sprite = MyGameManager.prefabData.LegL;
            someone.LegL.color = Color.white;
        }
        else if(parts == "LegR")
        {
            someone.LegR.sprite = MyGameManager.prefabData.LegR;
            someone.LegR.color = Color.white;
        }
    }
}
