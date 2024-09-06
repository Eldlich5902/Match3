using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class Board : MonoBehaviour
{
    public int width;
    public int height;

    public GameObject bgTilePrefab;

    public Fruit[] fruits;//mảng chứa loại của trái cây
    public Fruit[,] allFruits;//mảng chứa tọa độ trái cây

    public float fruitSpeed;

    [HideInInspector] 
    public MatchFinder matchFinder;

    //Preventing Player Input While Gems Move
    public enum BoardState//trạng thái của bảng
    {
        wait,
        move
    }
    public BoardState currentState = BoardState.move;

    public Fruit bomb;
    public float BombChance = 2f;

    [HideInInspector]
    public RoundManager roundManager;

    private float bonusMultiple;//bonus nhiều lần
    public float bonusAmount = 0.5f;//giá trị bonus

    private void Awake()
    {
        matchFinder = FindObjectOfType<MatchFinder>();
        roundManager = FindObjectOfType<RoundManager>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        allFruits = new Fruit[width, height];
        Setup();
        matchFinder = FindObjectOfType<MatchFinder>();
        //AudioManager.Instance.PlayMusic("Theme1");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
             ShuffleBoard();
        }
    }
    private void Setup()//setup bảng
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = new Vector2(x, y);
                GameObject bgTile = Instantiate(bgTilePrefab, pos, Quaternion.identity);//Quaternion.identity  giữ nguyên hướng ban đầu
                bgTile.transform.parent = transform;
                bgTile.name = "BG Tile - " + x + "," + y;

                int fruitToUse = Random.Range(0, fruits.Length);

                int interation = 0;//khởi tạo biến đếm bắt đầu từ 0
                while(MatchesAt(new Vector2Int(x, y), fruits[fruitToUse]) && interation < 100)
                {
                    fruitToUse = Random.Range(0, fruits.Length);//Tạo một chỉ số ngẫu nhiên (fruitToUse) để chọn một loại trái cây từ mảng fruits.
                    interation++;//theo dõi số lần lặp lại 
                    Debug.Log("Match found after " + interation);
                }
                SpawnFruit(new Vector2Int(x, y), fruits[fruitToUse]);
            }
        }
    }

    private void SpawnFruit(Vector2Int pos, Fruit fruitToSpawn)
    {
        if(Random.Range(0f,100f) < BombChance)//xác suất xuất hiện bom
        {
            fruitToSpawn = bomb;
        }

        //pos.y + height:fruit sẽ đucợ xuất hiện theo kiểu trượt từ trên xuống
        Fruit fruit = Instantiate(fruitToSpawn, new Vector3(pos.x, pos.y + height, 0f), Quaternion.identity);
        fruit.transform.parent = transform;
        fruit.name = "Fruit - " + pos.x + "," + pos.y;
        allFruits[pos.x, pos.y] = fruit;//lưu lai vị trí của fruit

        fruit.SetupFruit(pos, this);//fruit đang được khởi tạo với vị trí của nó và tham chiếu đến bảng mà nó thuộc về
    }

    bool MatchesAt(Vector2Int positionToCheck, Fruit fruitToCheck)//kiểm tra match tại 1 vị trí cụ thể
    {
        //kiểm tra xem có 2 vật thể cùng loại liền kề theo chiều ngang
        if (positionToCheck.x > 1)
        {
            if (allFruits[positionToCheck.x - 1, positionToCheck.y].fruitType == fruitToCheck.fruitType 
            && allFruits[positionToCheck.x - 2, positionToCheck.y].fruitType == fruitToCheck.fruitType)
            { 
                return true; 
            }
        }
        //kiểm tra xem có 2 vật thể cùng loại liền kề theo chiều dọc
        if (positionToCheck.y > 1)
        {
            if (allFruits[positionToCheck.x, positionToCheck.y - 1].fruitType == fruitToCheck.fruitType
            && allFruits[positionToCheck.x, positionToCheck.y - 2].fruitType == fruitToCheck.fruitType)
            {
                return true;
            }
        }
        return false;
    }

    public void DestroyMatchesFruitAt(Vector2Int pos)//hủy vật thể riêng lẻ
    {
        if (allFruits[pos.x, pos.y] != null)
        {
            if (allFruits[pos.x, pos.y].isMatched)
            {
                Instantiate(allFruits[pos.x, pos.y].destroyEffect, 
                new Vector2(pos.x, pos.y), Quaternion.identity);//hiệu ứng phá hủy

                Destroy(allFruits[pos.x, pos.y].gameObject);
                allFruits[pos.x, pos.y] = null;
            }
        }
    }

    public void DestroyMatches()//hủy các vật thể match
    {
        for (int i = 0; i < matchFinder.currentMatches.Count; i++)
        {
            if (matchFinder.currentMatches[i] != null)
            {
                ScoreCheck(matchFinder.currentMatches[i]);
                DestroyMatchesFruitAt(matchFinder.currentMatches[i].positionIndex);
                AudioManager.Instance.PlaySFX("Match");
            }
        }
        StartCoroutine(DecreaseRow());
    }

    private IEnumerator DecreaseRow()//hủy hàng
    {
        yield return new WaitForSeconds(0.2f);
        int nullCounter = 0;//đếm số vật thể null
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allFruits[x, y] == null)
                {
                    nullCounter++;
                }
                else if (nullCounter > 0)
                {
                    //các vật thể trên sẽ rơi xuống khi có các vị trí null bên dưới
                    allFruits[x, y].positionIndex.y -= nullCounter;//giảm vị trí y
                    allFruits[x, y - nullCounter] = allFruits[x, y];//lưu lại vị trí sau khi rơi
                    allFruits[x, y] = null;//vị trí trên sẽ trở thành null
                }
            }
            nullCounter = 0;
        }
        StartCoroutine(FillBoard());
    }

    private IEnumerator FillBoard()//làm đầy lại bảng
    {
        yield return new WaitForSeconds(0.5f);
        RefillBoard();

        yield return new WaitForSeconds(0.5f);
        matchFinder.FindAllMatches();//check các kết quả match

        if (matchFinder.currentMatches.Count > 0)//nếu có match thì phá hủy
        {
            bonusMultiple++;
            yield return new WaitForSeconds(1.25f);
            DestroyMatches();
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            //kiểm tra xem còn vị trí match không
            if (IsDeadLocked())
            {
                Debug.Log("DeadLock");
            }

            currentState = BoardState.move;
            bonusMultiple = 0f;
        }  
    }

    private void RefillBoard()//làm đầy lại bảng
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allFruits[x, y] == null)
                {
                    int fruitToUse = Random.Range(0, fruits.Length);
                    SpawnFruit(new Vector2Int(x, y), fruits[fruitToUse]);
                }
            }
        }
        CheckMisplaceFruit();
    }

    private void CheckMisplaceFruit()//Removing Duplicates(các fruit trùng vị trí)
    {
        List<Fruit> foundFruit = new List<Fruit>();
        foundFruit.AddRange(FindObjectsOfType<Fruit>());//tìm tất cả các fruit
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(foundFruit.Contains(allFruits[x, y]))
                {
                    foundFruit.Remove(allFruits[x, y]);//xóa các fruit trùng vị trí trong list
                }
            }
        }
        foreach (Fruit fruit in foundFruit)
        {
            Destroy(fruit.gameObject);//hủy fruit object tương ứng
        }
    }

    public void ShuffleBoard()//xào lại bàn chơi 
    {
        if (currentState != BoardState.wait)
        {
            currentState = BoardState.wait;
            List<Fruit> fruitFromBoard = new List<Fruit>();//danh sách lưu trữ các fruit mới trên bảng
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    fruitFromBoard.Add(allFruits[x, y]);//thêm vào danh fruitFromBoard
                    allFruits[x, y] = null;//đặt vị trí đó thành null
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int fruitToUse = Random.Range(0, fruitFromBoard.Count);

                    int iterations = 0;//vòng lặp bằng 0
                    
                    while (MatchesAt(new Vector2Int(x, y), fruitFromBoard[fruitToUse]) 
                    && fruitFromBoard.Count > 1 && iterations < 100)//số vòng lặp ko quá 100
                    {
                        fruitToUse = Random.Range(0, fruitFromBoard.Count);//chọn ngẫu nhiên một vật thể từ danh sách fruitFromBoard
                        iterations++;
                    }
                    fruitFromBoard[fruitToUse].SetupFruit(new Vector2Int(x, y), this);//trái cây tại chỉ mục ngẫu nhiên sẽ được gán cho vị trí hiện tại trên bảng
                    allFruits[x, y] = fruitFromBoard[fruitToUse];//lưu lại vị trí
                    fruitFromBoard.RemoveAt(fruitToUse);//xóa vị trí đã sử dụng
                }
            }
        }
        StartCoroutine(FillBoard());//làm đầy lại bảng
    }

    public void ScoreCheck(Fruit fruitToCheck)//kiểm tra điểm
    {
        roundManager.currentScore += fruitToCheck.scoreValue;
        if(bonusMultiple > 0)
        {
            float bonusToAdd = fruitToCheck.scoreValue * bonusMultiple * bonusAmount;//tính điểm bonus
            roundManager.currentScore += Mathf.RoundToInt(bonusToAdd);//làm tròn số
        }
    }
    //test
    private void SwitchPieces(int column, int row, Vector2 direction)//chuyển thử vị trí của 2 vật thể
    {
        //take second fruit and save it in a holder
        Fruit holder = allFruits[column + (int)direction.x, row + (int)direction.y] as Fruit;
        //switching the first fruit to the second position
        allFruits[column + (int)direction.x,row + (int)direction.y]= allFruits[column, row];
        //set the first fruit to the second fruit position
        allFruits[column,row] = holder;
    }
    private bool CheckForMatches()//kiểm tra match
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)//tý nhớ sửa lại bản gửi anh Trung
            {
                //check if the fruit is not null
                if (allFruits[i, j] != null)
                {
                    //xét tối đa 2 vật thể từ vật thể gốc chạm tới giới hạn chiều ngang của bảng
                    if (i < width - 2)
                    {
                        //check if the fruit to the right 1 and to the right 2 are not null
                        if (allFruits[i + 1, j] != null && allFruits[i + 2, j] != null)
                        {
                            //check if the fruitType match
                            if (allFruits[i + 1, j].fruitType == allFruits[i, j].fruitType 
                            && allFruits[i + 2, j].fruitType == allFruits[i, j].fruitType)
                            {
                                return true;
                            }
                        }
                    }
                    //xét tối đa 2 vật thể từ vật thể gốc chạm tới giới hạn chiều dọc của bảng
                    if (j < height - 2)
                    {
                        //check if the fruit 1 above and 2 above are not null
                        if (allFruits[i, j + 1] != null && allFruits[i, j + 2] != null)
                        {
                            //check if the fruitType match
                            if (allFruits[i, j + 1].fruitType == allFruits[i, j].fruitType
                            && allFruits[i, j + 2].fruitType == allFruits[i, j].fruitType)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)//kiểm tra khi chuyển vị trí
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }

    private bool IsDeadLocked()//kiểm tra khi không còn vị trí match
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allFruits[i, j] != null)
                {
                    if (i < width - 1)//kiểm tra xem có ô lân cận ở bên phải
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if (j < height - 1)//kiểm tra xem có ô lân cận ở phía trên
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }
}
