using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Fruit : MonoBehaviour
{
    public Vector2Int positionIndex;//vị trí lưu
    public Board board;//bảng tham chiếu

    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;

    private bool mousePressed;  
    private float swipeAngle = 0;//góc vuốt từ bên phải là 0, trên là 90, trái là 180, dưới là -90

    private Fruit otherFruit;

    public enum FruitType//lưu trữ tên các loại trái cây
    { APPLE, BANANA, BLUEBERRY, GRAPE, ORANGE, PEAR, STRAWBERRY, BOMB }

    public FruitType fruitType;

    public bool isMatched;
    
    public Vector2Int previousPosition;

    public GameObject destroyEffect;//hiệu ứng phá hủy

    public int blastSize = 1;//bán kính phá hủy

    public int scoreValue = 10;//điểm số

    private HintManager hintManager;

    // Start is called before the first frame update
    void Start()
    {
        hintManager = FindObjectOfType<HintManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //swap fruit
        if (Vector2.Distance(transform.position, positionIndex) > 0.01f)//nếu khoảng cách giữa vị trí hiện tại và vị trí lưu lớn hơn 0.01 thì di chuyển
        {
            //lerp: di chuyển từ vị trí này đến vị trí khác với các khoảng di chuyển nhỏ dần nhỏ dần
            transform.position = Vector2.Lerp(transform.position, positionIndex, board.fruitSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = new Vector3(positionIndex.x, positionIndex.y, 0f);//đặt vị trí mới
            board.allFruits[positionIndex.x, positionIndex.y] = this;
        }
        //mouse pressed
        if (mousePressed && Input.GetMouseButtonUp(0)) 
        {
            mousePressed = false;

            if (board.currentState == Board.BoardState.move 
            && board.roundManager.roundTime > 0)//nếu bảng đang di chuyển và thời gian còn
            {
                finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);//lấy vị trí chuột khi thả tay
                CalculateAngle();  
            }
        }
    }

    public void SetupFruit(Vector2Int pos, Board theBoard)//thiết lập trái cây
    {
        positionIndex = pos;//lưu vị trí
        board = theBoard;//lưu bảng
    }

    private void OnMouseDown()//lấy vị trí chuột khi click
    {
        //destroy hint
        if (hintManager != null){ hintManager.DestroyHint(); }
        
        //mouse pressed
        if (board.currentState == Board.BoardState.move && board.roundManager.roundTime > 0)//nếu bảng đang di chuyển và thời gian còn
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);//lấy vị trí chuột khi click
            AudioManager.Instance.PlaySFX("Swap");
            mousePressed = true;
            
        }
    }

    public void CalculateAngle()//tính góc vuốt
    {
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x);//tính góc vuốt
        swipeAngle = swipeAngle * 180 / Mathf.PI;//chuyển đổi radian sang độ
        Debug.Log(swipeAngle);

        if(Vector3.Distance(firstTouchPosition, finalTouchPosition) > 0.5f)//nếu khoảng cách giữa 2 điểm lớn hơn 0.5 thì di chuyển
        {
            MoveFruit();
        }
    }

    private void MoveFruit()//di chuyển trái cây
    {
        previousPosition = positionIndex;
        //vuốt sang phải
        if(swipeAngle > -45 && swipeAngle < 45 && positionIndex.x < board.width -1)
        {
            otherFruit = board.allFruits[positionIndex.x + 1, positionIndex.y];//vị trí trái cây được đổi chỗ
            otherFruit.positionIndex.x--;//vị trí trái cây bị đổi chỗ
            positionIndex.x++;
        }
        //vuốt lên trên
        else if(swipeAngle > 45 && swipeAngle <= 135 && positionIndex.y < board.height - 1)
        {
            otherFruit = board.allFruits[positionIndex.x, positionIndex.y + 1];//vị trí trái cây được đổi chỗ
            otherFruit.positionIndex.y--;//vị trí trái cây bị đổi chỗ
            positionIndex.y++;
        }
        //vuốt xuống dưới
        else if (swipeAngle < -45 && swipeAngle >= -135 && positionIndex.y > 0)
        {
            otherFruit = board.allFruits[positionIndex.x, positionIndex.y - 1];//vị trí trái cây được đổi chỗ
            otherFruit.positionIndex.y++;//vị trí trái cây bị đổi chỗ
            positionIndex.y--;
        }
        //vuốt sang trái
        else if (swipeAngle > 135 || swipeAngle < -135 && positionIndex.y > 0)
        {
            otherFruit = board.allFruits[positionIndex.x - 1, positionIndex.y];//vị trí trái cây được đổi chỗ
            otherFruit.positionIndex.x++;//vị trí trái cây bị đổi chỗ
            positionIndex.x--;
        }

        board.allFruits[positionIndex.x, positionIndex.y] = this;//lưu vị trí mới trái cây được đổi chỗ
        board.allFruits[otherFruit.positionIndex.x, 
        otherFruit.positionIndex.y] = otherFruit;//lưu vị trí mới trái cây bị đổi chỗ

        StartCoroutine(CheckMove());
    }

    //IEnumerator cho phép bỏ qua frame

    public IEnumerator CheckMove()//swap back if not match
    {
        board.currentState = Board.BoardState.wait;

        yield return new WaitForSeconds(0.5f);//chờ 0.5s

        board.matchFinder.FindAllMatches();//kiểm tra xem có match không

        if(otherFruit != null) 
        {
            if (!isMatched && !otherFruit.isMatched)//nếu không match thì đổi lại vị trí
            {
                otherFruit.positionIndex = positionIndex;
                positionIndex = previousPosition;
                
                board.allFruits[positionIndex.x, positionIndex.y] = this;//lưu vị trí mới trái cây được đổi chỗ
                board.allFruits[otherFruit.positionIndex.x, otherFruit.positionIndex.y] = otherFruit;//lưu vị trí mới trái cây bị đổi chỗ

                yield return new WaitForSeconds(0.5f);

                board.currentState = Board.BoardState.move;
            }
            else
            {
                board.DestroyMatches();//hủy match
            }
        }   
    }
}
