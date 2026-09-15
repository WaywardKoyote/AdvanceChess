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

    public int maxHealth;
    public int health;
    public int attackDamage;

    void Start()
    {
        stats = new UnitStats(Random.Range(0, 100), Random.Range(0, 100), Random.Range(0, 100), Random.Range(0, 100));
        movementRange = Mathf.RoundToInt(stats.speed * 0.1f);
        attackRange = Mathf.RoundToInt(stats.perception * 0.05f);
        attackRange = Mathf.Clamp(attackRange, 1, int.MaxValue);
        movementLeft = movementRange;
        maxHealth = 1 + Mathf.RoundToInt(stats.endurance * 0.25f);
        health = maxHealth;
        attackDamage = 1 + Mathf.RoundToInt(stats.strength * 0.05f);
    }
    
    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    public void MoveTo(List<Tile> newPath)
    {

        if (!owner.isPlayerTurn || newPath == null || newPath.Count == 0) return;

        Tile startTile = owner.gridManager.GetTile(gridPosition);
        startTile.isOccupied = false;
        Tile destinationTile = newPath[newPath.Count - 1];
        destinationTile.isOccupied = true;

        path = new List<Tile>(newPath);

        targetPosition = path[0].transform.position;
        gridPosition = path[0].gridPosition;
        isMoving = true;

        /* Refactored
        targetPosition = position;
        gridPosition = gridPos;
        isMoving = true;
        */
    }

    private void HandleMovement()
    {
        if (!isMoving || path == null || path.Count == 0) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if ((transform.position - targetPosition).sqrMagnitude < STOPPING_DISTANCE * STOPPING_DISTANCE)
        {
            transform.position = targetPosition;

            movementLeft -= path[0].moveCost;

            path.RemoveAt(0);

            if (path.Count > 0)
            {
                targetPosition = path[0].transform.position;
                gridPosition = path[0].gridPosition;
            }
            else
            {
                isMoving = false;
                owner.ChangeSelectedUnit(this);
            }
        }

        /* Refactored
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
        */
    }

    // Called when the mouse is clicked, changes the selected Unit to this Unit
    public void OnPointerDown (PointerEventData eventData)
    {
        if (owner.isPlayerTurn)
        {
            owner.ChangeSelectedUnit(this);
            return;
        }
        /* Refactored
        else
        {
            if (Player.selectedUnit)
            {
                Tile targetTile = owner.gridManager.GetTile(gridPosition);
                Tile attackerTile = owner.gridManager.GetTile(Player.selectedUnit.gridPosition);

                if (owner.gridManager.GetHeuristic(targetTile, attackerTile) > Player.selectedUnit.attackRange)
                {
                    Debug.Log("Out of Range");
                    return; //If the distance between our attackerTile and our targetTile is bigger than our attackRange, quit out.
                }

                Tile closestAttackTile = owner.gridManager.GetClosestAttackTile(this, Player.selectedUnit);
                //Player.selectedUnit.path = owner.gridManager.GetPath(attackerTile, closestAttackTile);

                if (Player.selectedUnit.path.Count == 0)
                {
                    Player.selectedUnit.Attack(this);
                }
                else
                {
                    Player.selectedUnit.MoveTo(closestAttackTile.transform.position, closestAttackTile.gridPosition);
                }
            }
        }
        */

        Unit playerUnit = Player.selectedUnit;

        if (playerUnit == null || playerUnit.isMoving) return;

        Tile unitTile = owner.gridManager.GetTile(gridPosition);
        Tile attackerTile = owner.gridManager.GetTile(playerUnit.gridPosition);

        int distance = owner.gridManager.GetHeuristic(unitTile, attackerTile);

        if (distance > playerUnit.attackRange) return;

        Tile closestAttackTile = owner.gridManager.GetClosestAttackTile(this, playerUnit);

        if (closestAttackTile == null)
        {
            playerUnit.Attack(this);
            return;
        }

        List<Tile> path = owner.gridManager.GetPath(attackerTile, closestAttackTile);

        if (path == null || path.Count == 0)
        {
            playerUnit.Attack(this);
        }
        else
        {
            playerUnit.MoveTo(path);
        }
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            owner.playerUnits.Remove(this);
            Destroy(gameObject);
        }
    }

    public void Attack(Unit target)
    {
        Tile attackerTile = owner.gridManager.GetTile(gridPosition);
        Tile targetTile = owner.gridManager.GetTile(target.gridPosition);

        if (owner.gridManager.GetHeuristic(attackerTile, targetTile) <= attackRange)
        {
            target.TakeDamage(attackDamage);
        }
    }
}
