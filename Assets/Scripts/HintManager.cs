using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    private Board board;
    public float hintDelay;
    private float hintDelaySeconds;
    public GameObject hintParticle;
    public GameObject currentHint;

    // Start is called before the first frame update
    void Start()
    {
        //Fruit fruitComponent = currentHint.GetComponent<Fruit>();
        board = FindObjectOfType<Board>();//tét thử
        hintDelaySeconds = hintDelay;
    }

    // Update is called once per frame
    void Update()
    {
        hintDelaySeconds -= Time.deltaTime;
        if(hintDelaySeconds <= 0 && currentHint == null)
        {
            MarkHint();
            hintDelaySeconds = hintDelay;
        }
    }
    //Find all possible matches on the board and show a hint
    List<Fruit> FindAllPossibleMatches()
    {
        List<Fruit> possibleMoves = new List<Fruit>();

        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allFruits[i, j] != null)
                {
                    if (i < board.width - 1)//kiểm tra xem có cặp có thể kết hợp ở phía bên phải của ô hiện tại
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.right))
                        {
                            possibleMoves.Add(board.allFruits[i, j]);//thêm vào danh sách các vật thể có thể match
                        }
                    }
                    if (j < board.height - 1)//kiểm tra xem có cặp có thể kết hợp ở phía trên của ô hiện tại
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.up))
                        {
                            possibleMoves.Add(board.allFruits[i, j]);//thêm vào danh sách các vật thể có thể match
                        }
                    }
                }
            }
        }
        return possibleMoves;
    }

    //pick one of the matches randomly
    Fruit PickOneRandomly()
    {
        List<Fruit> possibleMoves = new List<Fruit>();
        possibleMoves = FindAllPossibleMatches();
        if(possibleMoves.Count > 0) 
        {
            int pieceToUse = Random.Range(0, possibleMoves.Count);
            return possibleMoves[pieceToUse];
        }
        return null;
    }
    //create a hint at the chosen location
    private void MarkHint()
    {
        Fruit move = PickOneRandomly();
        if (move != null)
        {
            currentHint = Instantiate(hintParticle, move.transform.position, Quaternion.identity);//tạo hiệu ứng hint
        }
    }
    //remove hint
    public void DestroyHint()
    {
        if(currentHint != null)
        {
            Destroy(currentHint);
            currentHint = null;
            hintDelaySeconds = hintDelay;
        }
    }
}
