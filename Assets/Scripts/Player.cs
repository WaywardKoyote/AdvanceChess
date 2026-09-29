using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public string playerName;

    public static Unit selectedUnit;
    public static Unit hoverUnit;

    public GridManager gridManager;

    public List<Unit> playerUnits;

    public bool isPlayerTurn = false;
    public bool readyToEndTurn = false;
    public int movesRemaining = 1;

    public Color playerColor;
    public Color negativeColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gridManager == null) gridManager = FindAnyObjectByType<GridManager>();        
        SnapUnits();
        ResetUnits();
    }

    private void Update()
    {
        if (isPlayerTurn)
        {
            CheckUnits();
        }
    }

    public void ChangeSelectedUnit(Unit unit)
    {
        DeselectUnit();
        selectedUnit = unit;
        selectedUnit.healthBarVis.alpha = 1;
        Tile unitTile = gridManager.GetTile(unit.gridPosition);
        unitTile.SelectTile();
        gridManager.HighlightRange(unitTile, unit.movementLeft, unit.attackRange);
    }

    public void DeselectUnit()
    {
        if(selectedUnit != null) selectedUnit.healthBarVis.alpha = 0;
        selectedUnit = null;
    }

    private void SnapUnits()
    {
        foreach (Unit unit in playerUnits)
        {
            Tile unitTile = gridManager.GetTile(unit.transform.position);
            unit.gridPosition = unitTile.gridPosition;
            unit.transform.position = unitTile.transform.position + Vector3.up * 0.05f;
            unitTile.isOccupied = true;
            unit.owner = this;
        }
    }

    public void ResetUnits()
    {
        gridManager.ResetGridHighlights();

        foreach (Unit unit in playerUnits)
        {
            unit.spriteImage.color = unit.unitColor;
            unit.movementLeft = unit.movementRange;
            unit.attacksLeft = 1;
            unit.tileStartedTurnOn = gridManager.GetTile(unit.gridPosition);
        }
    }

    private void CheckUnits()
    {
        // if(movesRemaining <= 0) readyToEndTurn = true;

        foreach (Unit unit in playerUnits)
        {
            if (!unit.IsUnitExpended())
            {
                return;
            }
        }

        readyToEndTurn = true;
    }
}
