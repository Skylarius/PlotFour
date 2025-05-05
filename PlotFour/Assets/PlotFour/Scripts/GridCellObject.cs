using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCellObject : MonoBehaviour
{
    private Material InitialMaterial;
    public GameObject TouchingPlayer = null;

    private void Start()
    {
        InitialMaterial = GetComponent<MeshRenderer>().material;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Pawn"))
        {
            GetComponent<MeshRenderer>().material = other.GetComponent<MeshRenderer>().material;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pawn"))
        {
            GetComponent<MeshRenderer>().material = InitialMaterial;
        }
    }
}
