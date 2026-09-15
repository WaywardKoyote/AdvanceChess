using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using NUnit.Framework;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IComparable<Tile>
{
    public Vector2Int gridPosition;

    [Header("Colors")]
    public Color originalColor;
    public Color highlightColor = Color.yellow;
    public Color selectedColor = Color.blue;

    public Renderer tileRenderer;
    public static Tile selectedTile;
    public bool inMoveRange = false;
    public bool inAttackRange = false;

    public int moveCost = 0;

    public bool isOccupied = false;

    public int gCost = int.MaxValue;
    public int hCost = 0;
    public int fCost => gCost + hCost;

    public Tile parent;

    public GridManager gridManager;

    private void Start()
    {
        tileRenderer = GetComponentInChildren<Renderer>();
        ChangeColor(originalColor);
    }

    public void ChangeColor(Color newColor)
    {
        tileRenderer.material.color = newColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectedTile != this)
        {
            ChangeColor(highlightColor);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectedTile == this) return;
        
        if (inMoveRange)
        {
            ChangeColor(Color.cyan);
        }
        else if (inAttackRange)
        {
            ChangeColor(Color.red);
        }
        else
        {
            ChangeColor(originalColor);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Unit selected = Player.selectedUnit;

        if (selected == null || selected.isMoving) return;
        if (!inMoveRange) return;

        Tile startTile = gridManager.GetTile(selected.gridPosition);

        List<Tile> path = gridManager.GetPath(startTile, this, startTile);

        if (path == null || path.Count == 0) return;

        int totalCost = 0;

        foreach (Tile t in path) totalCost += t.moveCost;

        if (totalCost > selected.movementLeft) return;

        selected.MoveTo(path);

        SelectTile();

        gridManager.ResetGridHighlights();

        /* Deprecated
        if (Player.selectedUnit)
        {
            if (Player.selectedUnit.isMoving) return;
            if (inMoveRange)
            {
                gridManager.GetTile(Player.selectedUnit.gridPosition).isOccupied = false;
                Player.selectedUnit.path = gridManager.GetPath(gridManager.GetTile(Player.selectedUnit.gridPosition), this);
                Player.selectedUnit.MoveTo(Player.selectedUnit.path[0].transform.position, Player.selectedUnit.path[0].gridPosition);
                isOccupied = true;
            }
            else
            {
                Player.selectedUnit = null;
            }
        }

        SelectTile();
        gridManager.ResetGridHighlights();
        */

        /* DEPRECATED
        FindAnyObjectByType<GridManager>().HighlightMoveRange(this, FindAnyObjectByType<Unit>().movementRange);   //TODO Replace with a not cursed version of this line (FindAnyObjectByType is SLOW)
        */
    }

    public void SelectTile()
    {
        if (selectedTile)
        {
            selectedTile.ChangeColor(selectedTile.originalColor);
        }

        selectedTile = this;
        ChangeColor(selectedColor);
    }

    public int CompareTo(Tile other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0) compare = hCost.CompareTo(other.hCost);

        return compare;
    }
}
