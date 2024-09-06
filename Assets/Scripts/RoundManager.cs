using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RoundManager : MonoBehaviour
{
    public float roundTime = 60f;
    private UIManager uIManager;

    private bool endingRound = false;

    private Board board;

    public int currentScore;

    public float displayScore;

    public float scoreSpeed;

    public int scoreTarget1, scoreTarget2, scoreTarget3;
    // Start is called before the first frame update
    void Awake()
    {
        uIManager = FindObjectOfType<UIManager>();
        board = FindObjectOfType<Board>();
    }

    // Update is called once per frame
    void Update()
    {
        if(roundTime > 0) 
        {
            roundTime -= Time.deltaTime;//decrease time
            if(roundTime < 0)
            {
                roundTime = 0;
                endingRound = true;
                //AudioManager.Instance.PlayMusic("theme1");
            }
        }
        if(endingRound && board.currentState == Board.BoardState.move)//if the round is ending and the board is not moving
        {
            WinCheck();
            AudioManager.Instance.PlayMusic("GameOverMusic");//play the game over music
            endingRound = false;
        }
        uIManager.timeText.text = roundTime.ToString("0.0") + "s";

        displayScore = Mathf.Lerp(displayScore, currentScore, scoreSpeed * Time.deltaTime);//tăng điểm số dần dần
        uIManager.scoreText.text = displayScore.ToString("0");//hiển thị điểm số  
    }

    private void WinCheck()//kiểm tra kết quả của người chơi
    {
        uIManager.roundOverScreen.SetActive(true);//show the round over screen
        uIManager.winScore.text = currentScore.ToString();//show the final score
        if (currentScore >= scoreTarget3)
        {
            uIManager.winText.text = "You earned 3 Stars!";
            uIManager.winStar3.SetActive(true);
        }
        else if (currentScore >= scoreTarget2)
        {
            uIManager.winText.text = "You earned 2 Stars!";
            uIManager.winStar2.SetActive(true);
        }
        else if (currentScore >= scoreTarget1)
        {
            uIManager.winText.text = "You earned 1 Star!";
            uIManager.winStar1.SetActive(true);
        }
        else
        {
            uIManager.winText.text = "You earned 0 Stars!";
        }
    }
}
