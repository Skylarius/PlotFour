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

    [SerializeField]
    private GridCell[,] Grid;

    public int CurrentPlayer = 1;

    public bool IsGameRunning = false;

#if UNITY_EDITOR
    [SerializeField]
    private bool ShouldPlotDebugGrid = false;

    [TextArea(10,10)]
    public string GridDebug;
#endif

    public List<Tuple<int, int>> WinningSequence;

    public delegate void WinGameCallback(List<Tuple<int, int>> WinningSequence);
    public event WinGameCallback OnWinGame;

    // Start is called before the first frame update
    void Start()
    {
        IsGameRunning = true;
        WinningSequence = new List<Tuple<int, int>>();
        Grid = new GridCell[Rows, Columns];
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                Grid[i, j] = GridCell.Empty;
            }
        }
        // Test
        //StartCoroutine(TestCoroutineRandom());
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
#if UNITY_EDITOR
        PlotDebugGrid();
#endif
    }

    /// <summary>
    /// Insert pawn in grid
    /// </summary>
    /// <param name="Column"> Column to insert pawn in </param>
    /// <returns>Final position of the pawn (x=row, y=column) </returns>
    public Vector2 InsertPawn(int Column)
    {
        if (State != GameState.PlayerTurn)
        {
            return new Vector2(-1, -1);
        }
        if (IsColumnFull(Column))
        {
            return new Vector2(-1, -1);
        }
        for (int row = 0; row < Rows; row++)
        {
            if (row == Rows - 1 || Grid[row + 1, Column] != GridCell.Empty)
            {
                Grid[row, Column] = (CurrentPlayer == 1) ? GridCell.Player1 : GridCell.Player2;
                State = GameState.CheckFour;
                return new Vector2(row, Column);
            }
        }
        return new Vector2(-1, -1);
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
                    OnWinGame(WinningSequence);
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
        if (Row + 3 >= Rows)
        {
            return false;
        }
        WinningSequence.Add(new Tuple<int, int>(Row, Column));
        for (int i = 0; i < 3; i++)
        {
            if (Grid[Row + i, Column] != Grid[Row + i + 1, Column])
            {
                WinningSequence.Clear();
                return false;
            }
            WinningSequence.Add(new Tuple<int, int>(Row + i + 1, Column));
        }
        return true;
    }

    bool CheckFourHorizontal(int Row, int Column)
    {
        if (Column + 3 >= Columns)
        {
            return false;
        }
        WinningSequence.Add(new Tuple<int, int>(Row, Column));
        for (int j = 0; j < 3; j++)
        {
            if (Grid[Row, Column + j] != Grid[Row, Column + j + 1])
            {
                WinningSequence.Clear();
                return false;
            }
            WinningSequence.Add(new Tuple<int, int>(Row, Column + j + 1));
        }
        return true;
    }

    bool CheckFourDiagonalSE(int Row, int Column)
    {
        if (Column + 3 >= Columns || Row + 3 >= Rows)
        {
            return false;
        }
        WinningSequence.Add(new Tuple<int, int>(Row, Column));
        for (int k = 0; k < 3; k++)
        {
            if (Grid[Row + k, Column + k] != Grid[Row + k + 1, Column + k + 1])
            {
                WinningSequence.Clear();
                return false;
            }
            WinningSequence.Add(new Tuple<int, int>(Row + k + 1, Column + k + 1));
        }
        return true;
    }

    bool CheckFourDiagonalNE(int Row, int Column)
    {
        if (Column + 3 >= Columns || Row - 3 <= 0)
        {
            return false;
        }
        WinningSequence.Add(new Tuple<int, int>(Row, Column));
        for (int k = 0; k < 3; k++)
        {
            if (Grid[Row - k, Column + k] != Grid[Row - k - 1, Column + k + 1])
            {
                WinningSequence.Clear();
                return false;
            }
            WinningSequence.Add(new Tuple<int, int>(Row - k - 1, Column + k + 1));
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

#if UNITY_EDITOR
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
#endif
}
