using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;


public enum GameState 
{
    Start,
    PlayerTurn,
    CheckFour,
    End,
    Undefined
}

public enum GridCell
{
    Empty,
    Player1,
    Player2
}

public class GameManager : MonoBehaviour
{

    public GameState State;

    public int Rows = 9;
    public int Columns = 7;

    public GridCell[,] Grid;

    public int CurrentPlayer = 1;

    public bool IsGameRunning = false;

    [SerializeField]
    private bool ShouldPlotDebugGrid = false;

    [TextArea(10,10)]
    public string GridDebug;

    // Start is called before the first frame update
    void Start()
    {
        IsGameRunning = true;
        Grid = new GridCell[Rows, Columns];
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                Grid[i, j] = GridCell.Empty;
            }
        }
        // Test
        StartCoroutine(TestCoroutineRandom());
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsGameRunning)
        {
            return;
        }
        switch(State)
        {
            case GameState.Undefined:
                Debug.LogError("GameState is Undefined. Ending the game with no score result");
                State = GameState.End; 
                break;
            case GameState.Start:
                break;
            case GameState.PlayerTurn:
                break;
            case GameState.CheckFour:
                CheckFour();
                break;
            case GameState.End:
                Debug.Log("Player " + ((CurrentPlayer == 1) ? "R" : " Y") + " WON");
                IsGameRunning = false;
                break;
        }

        PlotDebugGrid();
    }

    public void InsertPawn(int Column)
    {
        if (State != GameState.PlayerTurn)
        {
            return;
        }
        if (IsColumnFull(Column))
        {
            return;
        }
        for (int row = 0; row < Rows; row++)
        {
            if (row == Rows - 1 || Grid[row + 1, Column] != GridCell.Empty)
            {
                Grid[row, Column] = (CurrentPlayer == 1) ? GridCell.Player1 : GridCell.Player2;
                State = GameState.CheckFour;
                break;
            }
        }
    }

    void CheckFour()
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                GridCell Player = Grid[row, column];
                if (Player == GridCell.Empty)
                {
                    continue;
                }
                if (
                    CheckFourVertical(row, column) || CheckFourHorizontal(row, column)
                    || CheckFourDiagonalSE(row, column) || CheckFourDiagonalNE(row, column)
                    )
                {
                    State = GameState.End;
                    return;
                }
            }
        }
        State = GameState.PlayerTurn;
        CurrentPlayer = (CurrentPlayer == 1) ? 2 : 1;
        return;
    }

    bool CheckFourVertical(int Row, int Column)
    {
        if (Row + 4 > Rows)
        {
            return false;
        }
        for (int i = 0; i < 3; i++)
        {
            if (Grid[Row + i, Column] != Grid[Row + i + 1, Column])
            {
                return false;
            }
        }
        return true;
    }

    bool CheckFourHorizontal(int Row, int Column)
    {
        if (Column + 4 > Columns)
        {
            return false;
        }
        for (int j = 0; j < 3; j++)
        {
            if (Grid[Row, Column + j] != Grid[Row, Column + j + 1])
            {
                return false;
            }
        }
        return true;
    }

    bool CheckFourDiagonalSE(int Row, int Column)
    {
        if (Column + 4 > Columns || Row + 4 > Rows)
        {
            return false;
        }
        for (int k = 0; k < 3; k++)
        {
            if (Grid[Row + k, Column + k] != Grid[Row + k + 1, Column + k + 1])
            {
                return false;
            }
        }
        return true;
    }

    bool CheckFourDiagonalNE(int Row, int Column)
    {
        if (Column + 4 > Columns || Row - 4 < 0)
        {
            return false;
        }
        for (int k = 0; k < 3; k++)
        {
            if (Grid[Row - k, Column + k] != Grid[Row - k - 1, Column + k + 1])
            {
                return false;
            }
        }
        return true;
    }

    public bool IsColumnFull(int Column)
    {
        if (Column > Columns)
        {
            Debug.LogError("Column is greater than maximum number");
            return true;
        }
        return Grid[0, Column] != GridCell.Empty;
    }

    void PlotDebugGrid()
    {
        if (!ShouldPlotDebugGrid)
        {
            return;
        }
        if (Grid == null)
        {
            return;
        }
        string newGridDebug = "";
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                string Cell = "";
                switch(Grid[i,j])
                {
                    case GridCell.Player1:
                        Cell = "R";
                        break;
                    case GridCell.Player2:
                        Cell = "Y";
                        break;
                    default:
                        Cell = "O";
                        break;
                }

                newGridDebug += "| " + Cell + " |";
            }
            newGridDebug += "\n";
        }
        GridDebug = newGridDebug;
    }

    public IEnumerator TestCoroutine()
    {
        Queue<int> Actions = new Queue<int>();
        Actions.Enqueue(0);
        Actions.Enqueue(1);
        Actions.Enqueue(0);
        Actions.Enqueue(1);
        Actions.Enqueue(0);
        Actions.Enqueue(1);
        Actions.Enqueue(2);
        Actions.Enqueue(0);
        Actions.Enqueue(4);
        Actions.Enqueue(2);
        Actions.Enqueue(2);
        Actions.Enqueue(3);
        Actions.Enqueue(2);
        Actions.Enqueue(2);
        Actions.Enqueue(2);

        while (Actions.Count > 0)
        {
            InsertPawn(Actions.Dequeue());
            yield return new WaitForSeconds(0.3f);
        }
    }

    public IEnumerator TestCoroutineRandom()
    {
        while (IsGameRunning)
        {
            yield return new WaitForSeconds(0.3f);
            InsertPawn(UnityEngine.Random.Range(0, Columns));
        }
    }
}
