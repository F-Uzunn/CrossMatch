using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : InstanceManager<GridGenerator>
{
    public List<GameObject> cellList;
    public GameObject spritePrefab;
    public int gridSize;
    public float gridSpace;

    private void OnEnable()
    {
        EventManager.AddHandler(GameEvent.OnCreateGrid, OnCreateGrid);
        EventManager.AddHandler(GameEvent.OnClearCellList, OnClearCellList);
    }

    private void OnDisable()
    {
        EventManager.RemoveHandler(GameEvent.OnCreateGrid, OnCreateGrid);
        EventManager.RemoveHandler(GameEvent.OnClearCellList, OnClearCellList);
    }

    [ContextMenu("CreateGrid")]
    void OnCreateGrid(object gridSze)
    {
        gridSize = (int)gridSze;
        FitCameraToGrid();

        float gridWidth = (1 + gridSpace) * gridSize - gridSpace;
        float gridHeight = (1 + gridSpace) * gridSize - gridSpace;

        Vector2 startPos = new Vector2(-gridWidth / 2f + 1 / 2f, gridHeight / 2f - 1 / 2f);

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector2 spawnPos = startPos + new Vector2((1 + gridSpace) * x, -(1 + gridSpace) * y);
                GameObject gridCell = Instantiate(spritePrefab, spawnPos, Quaternion.identity);
                cellList.Add(gridCell);
            }
        }
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

    void OnClearCellList()
    {
        foreach (var item in cellList)
        {
            Destroy(item);
        }
        cellList.Clear();
    }
}