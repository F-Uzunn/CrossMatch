using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : InstanceManager<GridGenerator>
{
    public List<GameObject> cellList;
    public GameObject spritePrefab;
    public int gridSize;
    public float gridSpace;

    private CellPiece[,] grid;
    public List<GameObject> willbeDeletedList;

    private void OnEnable()
    {
        EventManager.AddHandler(GameEvent.OnCreateGrid, OnCreateGrid);
        EventManager.AddHandler(GameEvent.OnClearCellList, OnClearCellList);
        EventManager.AddHandler(GameEvent.OnCheckNeighbours, OnCheckNeighbours);
        EventManager.AddHandler(GameEvent.OnCheckIfAddedToDeleteList, OnCheckIfAddedToDeleteList);
    }

    private void OnDisable()
    {
        EventManager.RemoveHandler(GameEvent.OnCreateGrid, OnCreateGrid);
        EventManager.RemoveHandler(GameEvent.OnClearCellList, OnClearCellList);
        EventManager.RemoveHandler(GameEvent.OnCheckNeighbours, OnCheckNeighbours);
        EventManager.RemoveHandler(GameEvent.OnCheckIfAddedToDeleteList, OnCheckIfAddedToDeleteList);
    }

    void OnCreateGrid(object gridSze)
    {
        gridSize = (int)gridSze;
        FitCameraToGrid();

        float gridWidth = (1 + gridSpace) * gridSize - gridSpace;
        float gridHeight = (1 + gridSpace) * gridSize - gridSpace;

        Vector2 startPos = new Vector2(-gridWidth / 2f + 1 / 2f, gridHeight / 2f - 1 / 2f);

        grid = new CellPiece[gridSize, gridSize];

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector2 spawnPos = startPos + new Vector2((1 + gridSpace) * x, -(1 + gridSpace) * y);
                GameObject gridCell = Instantiate(spritePrefab, spawnPos, Quaternion.identity);
                cellList.Add(gridCell);
                gridCell.GetComponent<CellPiece>().columnCount = x;
                gridCell.GetComponent<CellPiece>().rowCount = y;
                grid[y, x] = gridCell.GetComponent<CellPiece>();
            }
        }
    }

    void OnClearCellList()
    {
        foreach (var item in cellList)
        {
            Destroy(item);
        }
        cellList.Clear();
    }

    void OnCheckIfAddedToDeleteList(object CellObject)
    {
        GameObject cellObj = (GameObject)CellObject;
        if (!willbeDeletedList.Contains(cellObj))
            willbeDeletedList.Add(cellObj);
    }

    void FitCameraToGrid()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        float gridWidth = (1 + gridSpace) * gridSize - gridSpace;
        float gridHeight = (1 + gridSpace) * gridSize - gridSpace;

        float screenHeight = 2f * mainCamera.orthographicSize;
        float screenWidth = screenHeight * mainCamera.aspect;

        float canvasScale = Mathf.Min(screenHeight / gridHeight, screenWidth / gridWidth);

        mainCamera.orthographicSize = screenHeight / (1.75f * canvasScale);
        mainCamera.transform.position = new Vector3(0, 0, -10f);
    }

    [ContextMenu("CHECKneighbour")]
    private void OnCheckNeighbours()
    {
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
}