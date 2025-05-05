using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SquareCellSettings
{
    public float SideLength;
    public float Offset;
    public GameObject Prefab;
}

public class SquareCell
{
    public int Row, Column;
    public Vector3 Position;
    public GameObject GameObject;
    public GameObject AssignedPawn;

    public SquareCell(int row, int column, Vector3 position, GameObject gameObject) 
    {
        Row = row;
        Column = column;
        Position = position;
        GameObject = gameObject;
        AssignedPawn = null;
    }
}

public class GridController : MonoBehaviour
{
    [SerializeField]
    public SquareCellSettings CellSettings;

    [SerializeField]
    private GameManager GameManager;

    [SerializeField]
    private List<SquareCell> CellList;

    // Start is called before the first frame update
    void Start()
    {
        CellSettings.Prefab.SetActive(false);
        CellList = new List<SquareCell>();
        Vector3 Centre = transform.position;
        float CellTotalSpace = CellSettings.SideLength + 2*CellSettings.Offset;
        Vector2 GridSize = new Vector2(CellTotalSpace * GameManager.Columns, CellTotalSpace * GameManager.Rows);
        for (int j = 0; j < GameManager.Columns; j++)
        {
            for (int i = 0; i < GameManager.Rows; i++)
            {
                Vector3 CellPosition = transform.position + Vector3.left * GridSize.x/2  + Vector3.up * GridSize.y;
                CellPosition += Vector3.right * CellTotalSpace * (j + 0.5f) + Vector3.down * CellTotalSpace * (i + 0.5f);
                GameObject CellObject = Instantiate(CellSettings.Prefab, transform);
                CellObject.name = string.Format("Cell [{0}, {1}]", j, i);
                CellObject.SetActive(true);
                CellObject.transform.position = CellPosition;
                CellList.Add(new SquareCell(i, j, CellPosition, CellObject));
            }
        }
    }

    public SquareCell GetCellFromGameObject(GameObject cellObject)
    {
        return CellList.Find((SquareCell Cell) => Cell.GameObject == cellObject);
    }

    public SquareCell GetCellFromCoordinates(int Row, int Column)
    {
        return CellList.Find((SquareCell Cell) => Cell.Row == Row && Cell.Column == Column);
    }
}
