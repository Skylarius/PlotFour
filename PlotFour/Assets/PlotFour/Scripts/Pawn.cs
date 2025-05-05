using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    public Vector3 InitialPosition;
    private Vector3 InitialScale;

    public bool isSelected = false;
    private bool isWinningPawn = false;

    public int PlayerNumber = 1;

    [SerializeField] GameManager GameManager;

    public GameObject SelectedCell = null;

    public Transform PlacedInCell = null;

    public int Row = -1;
    public int Column = -1;

    [SerializeField] float Speed = 10;

    // Start is called before the first frame update
    private void Start()
    {
        InitialScale = transform.localScale;
        GameManager.OnWinGame += OnWin;
    }

    void OnWin(List<Tuple<int, int>> CoordinateList)
    {
        Tuple<int, int> Found = CoordinateList.Find((Tuple<int, int> Coordinate) => Coordinate.Item1 == Row && Coordinate.Item2 == Column);
        if (Found != null)
        {
            isWinningPawn = true;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (PlacedInCell)
        {
            transform.position = Vector3.Lerp(transform.position, PlacedInCell.position, Time.deltaTime * Speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(PlacedInCell.up), Time.deltaTime * Speed);
        }
        if (isWinningPawn)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, InitialScale * 2, Time.deltaTime * Speed);
        }
    }

    public void OnSelectExit()
    {

        if (PlacedInCell)
        {
            return;
        }
        //Instantiate a copy
        GameObject Replacement = Instantiate(this.gameObject);
        Replacement.transform.position = InitialPosition;

        if (GameManager.CurrentPlayer != this.PlayerNumber)
        {
            return;
        }

        if (SelectedCell)
        {
            GridController GridController = SelectedCell.GetComponentInParent<GridController>();
            if (GridController == null)
            {
                return;
            }

            SquareCell SquareCell = GridController.GetCellFromGameObject(SelectedCell);

            if (SquareCell == null)
            {
                return;
            }

            Vector2 GridCoordinate = GameManager.InsertPawn(SquareCell.Column);

            if (GridCoordinate.x > -1 && GridCoordinate.y > -1)
            {
                SquareCell FinalSquareCell = GridController.GetCellFromCoordinates((int)GridCoordinate.x, (int)GridCoordinate.y);
                if (FinalSquareCell != null)
                {
                    PlacedInCell = FinalSquareCell.GameObject.transform;
                    Row = FinalSquareCell.Row;
                    Column = FinalSquareCell.Column;
                    FinalSquareCell.AssignedPawn = this.gameObject;
                    PlacedInCell.GetComponent<MeshRenderer>().enabled = false;
                    GetComponent<Rigidbody>().isKinematic = true;
                    GetComponent<Collider>().enabled = false;
                }
            }
            SelectedCell = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cell"))
        {
            SelectedCell = other.gameObject;
        } 
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cell") && other.gameObject == SelectedCell)
        {
            SelectedCell = null;
        }
    }
}
