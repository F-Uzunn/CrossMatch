using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : InstanceManager<GridManager>
{
    public List<GameObject> cellList;
    public CellPiece[,] grid;
    public List<GameObject> willbeDeletedList;

    private void OnEnable()
    {
        EventManager.AddHandler(GameEvent.OnCheckNeighbours, OnCheckNeighbours);
        EventManager.AddHandler(GameEvent.OnCheckIfAddedToDeleteList, OnCheckIfAddedToDeleteList);
        EventManager.AddHandler(GameEvent.OnClearCellList, OnClearCellList);
    }

    private void OnDisable()
    {
        EventManager.RemoveHandler(GameEvent.OnCheckNeighbours, OnCheckNeighbours);
        EventManager.RemoveHandler(GameEvent.OnCheckIfAddedToDeleteList, OnCheckIfAddedToDeleteList);
        EventManager.RemoveHandler(GameEvent.OnClearCellList, OnClearCellList);
    }

    void OnCheckIfAddedToDeleteList(object CellObject)
    {
        GameObject cellObj = (GameObject)CellObject;
        if (!willbeDeletedList.Contains(cellObj))
            willbeDeletedList.Add(cellObj);
    }

    [ContextMenu("CHECKneighbour")]
    private void OnCheckNeighbours()
    {
        int gridSize = GetComponentInParent<GridGenerator>().gridSize;
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                CellPiece cell = grid[row, col];
                cell.neighbors.Clear();

                //top neighbour
                if (row - 1 >= 0 && grid[row - 1, col].gameObject.GetComponent<CellPiece>().isCellSelected)
                    cell.neighbors.Add(grid[row - 1, col]);

                //bottom neighbor
                if (row + 1 < gridSize && grid[row + 1, col].gameObject.GetComponent<CellPiece>().isCellSelected)
                    cell.neighbors.Add(grid[row + 1, col]);

                //left neighbor
                if (col - 1 >= 0 && grid[row, col - 1].gameObject.GetComponent<CellPiece>().isCellSelected)
                    cell.neighbors.Add(grid[row, col - 1]);

                //right neighbor
                if (col + 1 < gridSize && grid[row, col + 1].gameObject.GetComponent<CellPiece>().isCellSelected)
                    cell.neighbors.Add(grid[row, col + 1]);
            }
        }
        EventManager.Broadcast(GameEvent.OnAddNeighboursToDeleteList);
        Invoke("DestroyNeighbours", 0.05f);
    }

    public void DestroyNeighbours()
    {
        if (willbeDeletedList.Count < 3)
            return;
        foreach (var item in willbeDeletedList)
        {
            item.GetComponent<CellPiece>().isCellSelected = false;
            item.transform.GetChild(0).gameObject.SetActive(false);
        }
        willbeDeletedList.Clear();

        EventManager.Broadcast(GameEvent.OnUpdateMatchCount);
    }

    void OnClearCellList()
    {
        foreach (var item in cellList)
        {
            Destroy(item);
        }
        cellList.Clear();
    }
}
