using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
// 메쉬프로 //

public class ScoreText : MonoBehaviour
{
    [SerializeField] int TotalScore = 0;
    public int getScore { get { return TotalScore; } set { TotalScore = value; } }
    TextMeshPro TextPosition;

    
    // Start is called before the first frame update

    private void Awake()
    {
        TextPosition = this.gameObject.GetComponent<TextMeshPro>();
        TextPosition.text = "0";
    }

    public void UpdateScore()
    {
        TextPosition.text = TotalScore.ToString();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
