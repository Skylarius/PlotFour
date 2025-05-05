using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    public Vector3 InitialPosition;
    private Vector3 InitialScale;

    public bool isSelected = false;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (PlacedInCell)
        {
            transform.position = Vector3.Lerp(transform.position, PlacedInCell.position, Time.deltaTime * Speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(PlacedInCell.up), Time.deltaTime * Speed);
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

            SquareCell? SquareCell = GridController.GetCellFromGameObject(SelectedCell);

            if (SquareCell == null)
            {
                return;
            }

            Vector2 GridCoordinate = GameManager.InsertPawn(SquareCell.Value.Column);

            if (GridCoordinate.x > -1 && GridCoordinate.y > -1)
            {
                SquareCell? FinalSquareCell = GridController.GetCellFromCoordinates((int)GridCoordinate.x, (int)GridCoordinate.y);
                if (FinalSquareCell != null)
                {
                    PlacedInCell = FinalSquareCell.Value.GameObject.transform;
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
