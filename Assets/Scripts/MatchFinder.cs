using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;//thư viện dùng để sử dụng List

public class MatchFinder : MonoBehaviour
{
    private Board board;
    public List<Fruit> currentMatches = new List<Fruit>();

    private void Awake()
    {
        board = FindObjectOfType<Board>();
    }
    public void FindAllMatches()//tìm tất cả các vật thể match
    {
        currentMatches.Clear();
        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                Fruit currentFruit = board.allFruits[x, y];
                if (currentFruit != null) {
                    if (x > 0 && x < board.width - 1)//kiểm tra xem có 2 vật thể cùng loại ở 2 bên trái phải không
                    {
                        Fruit leftFruit = board.allFruits[x - 1, y];
                        Fruit rightFruit = board.allFruits[x + 1, y];
                        if (leftFruit != null && rightFruit != null)
                        {
                            if (leftFruit.tag == currentFruit.tag && rightFruit.tag == currentFruit.tag)
                            {
                                if(leftFruit.fruitType == currentFruit.fruitType 
                                && rightFruit.fruitType == currentFruit.fruitType)//kiểm tra xem 3 vật thể cùng loại không
                                {
                                    leftFruit.isMatched = true;
                                    rightFruit.isMatched = true;
                                    currentFruit.isMatched = true;
                                    //thêm các vật thể hiện tại vào danh sách  match
                                    currentMatches.Add(currentFruit);
                                    currentMatches.Add(rightFruit);
                                    currentMatches.Add(leftFruit);
                                }
                            }
                        }
                    }
                    if (y > 0 && y < board.height - 1)//kiểm tra xem có 2 vật thể cùng loại ở 2 bên trên và dưới không
                    {
                        Fruit aboveFruit = board.allFruits[x, y + 1];
                        Fruit belowFruit = board.allFruits[x, y - 1];
                        if (aboveFruit != null && belowFruit != null)
                        {
                            if (aboveFruit.tag == currentFruit.tag && belowFruit.tag == currentFruit.tag)
                            {
                                if (aboveFruit.fruitType == currentFruit.fruitType 
                                && belowFruit.fruitType == currentFruit.fruitType)//kiểm tra xem 3 vật thể cùng loại không
                                {
                                    aboveFruit.isMatched = true;
                                    belowFruit.isMatched = true;
                                    currentFruit.isMatched = true;
                                    //thêm các vật thể hiện tại vào danh sách  match
                                    currentMatches.Add(currentFruit);
                                    currentMatches.Add(aboveFruit);
                                    currentMatches.Add(belowFruit);
                                }
                            }
                        }
                    }
                }
            }
        }
        if(currentMatches.Count > 0)
        {
            currentMatches = currentMatches.Distinct().ToList();//distinct loại bỏ các vật thể trùng nhau
        }
        CheckForBombs();
    }

    public void CheckForBombs()//kiểm tra vật thể nổ
    {
        for (int i = 0; i < currentMatches.Count; i++)
        {
            Fruit fruit = currentMatches[i];

            int x = fruit.positionIndex.x;
            int y = fruit.positionIndex.y;

            if (fruit.positionIndex.x > 0)//kiểm tra vật thể nổ ở bên trái
            {
                if(board.allFruits[x - 1, y] != null)
                {
                   if(board.allFruits[x - 1, y].fruitType == Fruit.FruitType.BOMB)
                   {
                       MarkBombArea(new Vector2Int(x - 1, y), board.allFruits[x - 1, y]);//đánh dấu vùng nổ của vật thể nổ
                   } 
                }
            }

            if (fruit.positionIndex.x < board.width - 1)//kiểm tra vật thể nổ ở bên phải
            {
                if (board.allFruits[x + 1, y] != null)
                {
                    if (board.allFruits[x + 1, y].fruitType == Fruit.FruitType.BOMB)
                    {
                        MarkBombArea(new Vector2Int(x + 1, y), board.allFruits[x + 1, y]);//đánh dấu vùng nổ của vật thể nổ
                    }
                }
            }

            if (fruit.positionIndex.y > 0)//kiểm tra vật thể nổ ở bên dưới
            {
                if (board.allFruits[x, y - 1] != null)
                {
                    if (board.allFruits[x, y - 1].fruitType == Fruit.FruitType.BOMB)
                    {
                        MarkBombArea(new Vector2Int(x, y - 1), board.allFruits[x, y - 1]);//đánh dấu vùng nổ của vật thể nổ
                    }
                }
            }

            if (fruit.positionIndex.y < board.height - 1)//kiểm tra vật thể nổ ở bên trên
            {
                if (board.allFruits[x, y + 1] != null)
                {
                    if (board.allFruits[x, y + 1].fruitType == Fruit.FruitType.BOMB)
                    {
                        MarkBombArea(new Vector2Int(x, y + 1), board.allFruits[x, y + 1]);//đánh dấu vùng nổ của vật thể nổ
                    }
                }
            }
        }
    }

    public void MarkBombArea(Vector2Int bombPos, Fruit theBomb)//đánh dấu vùng nổ của vật thể nổ
    {
        for (int x = bombPos.x - theBomb.blastSize; x <= bombPos.x + theBomb.blastSize; x++)//kiểm tra vùng nổ theo chiều ngang
        {
            for(int y = bombPos.y - theBomb.blastSize; y <= bombPos.y + theBomb.blastSize; y++)//kiểm tra vùng nổ theo chiều dọc
            {
                if(x >= 0 && x < board.width && y >= 0 && y < board.height)
                {
                    if(board.allFruits[x, y] != null)
                    {
                        board.allFruits[x, y].isMatched = true;
                        currentMatches.Add(board.allFruits[x, y]);//thêm vào danh sách match
                    }
                }
            }
        }
        currentMatches = currentMatches.Distinct().ToList();//distinct loại bỏ các vật thể trùng nhau
    }
}
