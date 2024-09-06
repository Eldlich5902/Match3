using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text timeText;
    public TMP_Text scoreText;

    public TMP_Text winScore;
    public TMP_Text winText;

    public GameObject winStar1,winStar2, winStar3;

    public GameObject roundOverScreen;

    // Start is called before the first frame update
    void Start()
    {
        winStar1.SetActive(false);
        winStar2.SetActive(false);
        winStar3.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
