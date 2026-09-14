using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Unit : MonoBehaviour, IPointerDownHandler
{
    public Vector2Int gridPosition;
    public float moveSpeed = 5f;
    public bool isMoving = false;
    private Vector3 targetPosition;

    private const float STOPPING_DISTANCE = 0.01f;

    public UnitStats stats;

    public int movementRange = 3;
    public int attackRange = 1;
    public int movementLeft;

    public Player owner;

    public List<Tile> path;

    void Start()
    {
        stats = new UnitStats(Random.Range(0, 100), Random.Range(0, 100));
        movementRange = Mathf.RoundToInt(stats.speed * 0.1f);
        attackRange = Mathf.RoundToInt(stats.perception * 0.05f);
        attackRange = Mathf.Clamp(attackRange, 1, int.MaxValue);
        movementLeft = movementRange;
    }
    
    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    public void MoveTo(Vector3 position, Vector2Int gridPos)
    {

        if (!owner.isPlayerTurn)
        {
            return;
        }
        targetPosition = position;
        gridPosition = gridPos;
        isMoving = true;
    }

    private void HandleMovement()
    {
        if (!isMoving) return;  //Don't waste resources if not moving

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);   //Move to new position over time

        if (Vector3.Distance(transform.position, targetPosition) < STOPPING_DISTANCE)
        {
            transform.position = targetPosition;
            isMoving = false;

            movementLeft -= path[0].moveCost;
            path.RemoveAt(0);

            if (path.Count > 0)
            {
                MoveTo(path[0].transform.position, path[0].gridPosition);
            }
            else
            {
                owner.ChangeSelectedUnit(this);
            }
        }
    }

    // Called when the mouse is clicked, changes the selected Unit to this Unit
    public void OnPointerDown (PointerEventData eventData)
    {
        if (owner.isPlayerTurn)
        {
            owner.ChangeSelectedUnit(this);
        }
        else
        {
            if (Player.selectedUnit)
            {
                Tile closestAttackTile = owner.gridManager.GetClosestAttackTile(this, Player.selectedUnit);

                Player.selectedUnit.MoveTo(closestAttackTile.transform.position, closestAttackTile.gridPosition);
            }
        }
    }
}
