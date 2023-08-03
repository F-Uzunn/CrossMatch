using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellPiece : MonoBehaviour
{
    public GameObject crossObject;
    public bool isCellSelected;

    public int columnCount;
    public int rowCount;

    public List<CellPiece> neighbors;

    private void OnEnable()
    {
        EventManager.AddHandler(GameEvent.OnAddNeighboursToDeleteList, OnAddNeighboursToDeleteList);
    }

    private void OnDisable()
    {
        EventManager.RemoveHandler(GameEvent.OnAddNeighboursToDeleteList, OnAddNeighboursToDeleteList);
    }
    void Awake()
    {
        crossObject = transform.GetChild(0).gameObject;
        crossObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        isCellSelected = !isCellSelected;
        crossObject.SetActive(isCellSelected);
        EventManager.Broadcast(GameEvent.OnCheckNeighbours);
    }

    private void OnAddNeighboursToDeleteList()
    {
        if (neighbors.Count >= 2 && isCellSelected)
        {
            List<CellPiece> neighBours = neighbors;

            EventManager.Broadcast(GameEvent.OnCheckIfAddedToDeleteList, this.gameObject);
            for (int i = 0; i < neighBours.Count; i++)
            {
                foreach (var item in neighBours)
                {
                    EventManager.Broadcast(GameEvent.OnCheckIfAddedToDeleteList, item.gameObject);
                }
            }
        }
    }
}
