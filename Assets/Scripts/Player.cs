using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public static Unit selectedUnit;
    public static GridManager gridManager;

    public List<Unit> playerUnits;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();
        SnapUnits();
    }

    public void ChangeSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        gridManager.HighlightMoveRange(gridManager.GetTile(unit.gridPosition), unit.movementRange);
    }

    private void SnapUnits()
    {
        foreach (Unit unit in playerUnits)
        {
            Tile unitTile = gridManager.GetTile(unit.transform.position);
            unit.gridPosition = unitTile.gridPosition;
            unit.transform.position = unitTile.transform.position + Vector3.up * 0.05f;
            unitTile.isOccupied = true;
        }
    }
}
