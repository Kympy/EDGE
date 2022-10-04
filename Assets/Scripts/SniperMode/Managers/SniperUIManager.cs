using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class SniperUIManager : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI EnemyPlayerName = null;

    [SerializeField] private GameObject TabPanel = null;
    [SerializeField] private TextMeshProUGUI User1Name = null;
    [SerializeField] private TextMeshProUGUI User2Name = null;
    [SerializeField] private TextMeshProUGUI User1State = null;
    [SerializeField] private TextMeshProUGUI User2State = null;

    [SerializeField] private CanvasGroup BloodEffect = null;
    private Coroutine bloodCoroutine = null;
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
        BloodEffect.alpha = 0f;
        EnemyPlayerName.text = "";
        User1Name.text = PhotonNetwork.NickName;
        User2Name.text = "";
        User1State.text = "";
        User2State.text = "";
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
        MyHP.text = "100 / 100";
    }
    public void UpdateHP(float current, float max)
    {
        MyHP.text = current + " / " + max;
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
    [PunRPC]
    public void UpdateIndicator(string parts, int playerIndex)
    {
        //if (photonView.IsMine == false) return;
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
            someone.ArmL.sprite = MyGameManager.prefabData.ArmLRed;
            someone.ArmL.color = Color.white;
        }
        else if(parts == "ArmR")
        {
            someone.ArmR.sprite = MyGameManager.prefabData.ArmRRed;
            someone.ArmR.color = Color.white;
        }
        else if(parts == "LegL")
        {
            someone.LegL.sprite = MyGameManager.prefabData.LegLRed;
            someone.LegL.color = Color.white;
        }
        else if(parts == "LegR")
        {
            someone.LegR.sprite = MyGameManager.prefabData.LegRRed;
            someone.LegR.color = Color.white;
        }
    }
    public void ShowBlood()
    {
        if(bloodCoroutine != null)
        {
            StopCoroutine(bloodCoroutine);
            bloodCoroutine = null;
        }
        bloodCoroutine = StartCoroutine(BloodUI());
    }
    private IEnumerator BloodUI()
    {
        BloodEffect.alpha = 1f;
        yield return new WaitForSecondsRealtime(1f);
        while(true)
        {
            BloodEffect.alpha -= 0.05f;
            if(BloodEffect.alpha < 0f)
            {
                BloodEffect.alpha = 0f;
                yield break;
            }
            yield return null;
        }
    }
    [PunRPC]
    public void ProcessGameExit()
    {
        StartCoroutine(GameExitUpdate());
    }
    public IEnumerator GameExitUpdate()
    {
        PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
        User2State.text = "DISCONNECTED";
        User2State.color = Color.red;

        User1State.text = "WIN";

        TabPanel.SetActive(true);

        yield return new WaitForSecondsRealtime(3f);
        PhotonNetwork.LoadLevel("RoomScene");
    }
    [PunRPC]
    public void ProcessGameEnd(ODINAPIHandler.Winner winner)
    {
        StartCoroutine(GameEnd(winner));
    }
    public IEnumerator GameEnd(ODINAPIHandler.Winner winner)
    {
        switch(winner)
        {
            case ODINAPIHandler.Winner.Me:
                {
                    User1State.text = "WIN";
                    User2State.text = "LOSE";
                    break;
                }
            case ODINAPIHandler.Winner.Other:
                {
                    User1State.text = "LOSE";
                    User2State.text = "WIN";
                    break;
                }
            default: { Debug.Log("UIManager : Winner declare error"); break; }
        }
        TabPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(5f);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel("RoomScene");
    }
}
