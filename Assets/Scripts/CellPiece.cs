using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellPiece : MonoBehaviour
{
    public GameObject crossObject;
    public bool isCellSelected;

    public int columnCount;
    public int rowCount;
    void Awake()
    {
        crossObject = transform.GetChild(0).gameObject;
        crossObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        isCellSelected = !isCellSelected;
        crossObject.SetActive(isCellSelected);
    }
}
