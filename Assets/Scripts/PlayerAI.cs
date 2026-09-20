using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerAI : MonoBehaviour
{
    private Player player;
    private GridManager gridManager;
    public List<Unit> units;
    public Unit activeUnit;
    public Unit targetUnit;

    public enum AIBehaviours
    {
        Random,
        Aggressive,
        Defensive,
        Smart,
        Dumb
    }

    public AIBehaviours behaviour = AIBehaviours.Random;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Player>();
        gridManager = player.gridManager;
        units.AddRange(player.playerUnits);
        activeUnit = units[0];

        foreach (Unit unit in units)
        {
            unit.isAI = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player.isPlayerTurn)
        {
            CheckBehaviour();
            HandleUnitTurn();
            EndActiveUnitTurn();
        }
    }

    // Find an Enemy Unit by health
    private Unit GetTargetByHealth(bool findLowest = true)
    {
        Unit[] enemyUnits = FindObjectsByType<Unit>(FindObjectsSortMode.None);  // Creat an array of all units

        int health = findLowest ? int.MaxValue : int.MaxValue;  // Set starting value based on if we are looking for highest or lowest health
        Unit targetUnit = null;

        foreach (Unit unit in enemyUnits)   // Go through all units
        {
            if (unit.owner == player) continue; // Skip if unit belongs to the current player

            if (findLowest && unit.health < health)
            {
                health = unit.health;
                targetUnit = unit;
            }
            else if (!findLowest && unit.health > health)
            {
                health = unit.health;
                targetUnit = unit;
            }
        }

        return targetUnit;
    }

    // Find an Enemy Unit by distance
    private Unit GetTargetByDistance(Unit fromUnit, bool findClosest = true)
    {
        Unit[] enemyUnits = FindObjectsByType<Unit>(FindObjectsSortMode.None);  // Creat an array of all units

        int distance = findClosest ? int.MaxValue : int.MinValue;   // Set starting value based on if we are looking for closest or furthest distance
        Unit targetUnit = null;

        foreach (Unit unit in enemyUnits)   // Go through all units
        {
            if (unit.owner == player) continue; // Skip if unit belongs to the current player

            int currentDistance = gridManager.GetHeuristic(gridManager.GetTile(fromUnit.gridPosition), gridManager.GetTile(unit.gridPosition));

            if (findClosest && distance > currentDistance)
            {
                distance = currentDistance;
                targetUnit = unit;
            }
            else if (!findClosest && distance < currentDistance)
            {
                distance = currentDistance;
                targetUnit = unit;
            }
        }

        return targetUnit;
    }

    // Find a random Enemy Unit
    private Unit GetRandomTarget()
    {
        Unit[] enemyUnits = FindObjectsByType<Unit>(FindObjectsSortMode.None);  // Creat an array of all units
        List<Unit> possibleTargets = new List<Unit>();

        foreach (Unit unit in enemyUnits)   // Go through all units
        {
            if (unit.owner != player) // If unit doesn't belongs to the current player, add it to the List
            {
                possibleTargets.Add(unit);
            }
        }

        if (possibleTargets.Count == 0) return null;    // If no possible targets then null
        int randomIndex = Random.Range(0, possibleTargets.Count);   // Pick a random target

        return possibleTargets[randomIndex];
    }

    private Tile GetFurthestTileFromEnemy()
    {
        Unit[] enemyUnits = FindObjectsByType<Unit>(FindObjectsSortMode.None);  // Creat an array of all units
        List<Unit> enemyList = new List<Unit>();

        foreach (Unit unit in enemyUnits)
        {
            if (unit.owner != player)
            {
                enemyList.Add(unit);
            }
        }

        int maxDistance = int.MinValue;
        Tile furthestTile = null;

        foreach (Tile tile in gridManager.map)
        {
            if (!tile.inMoveRange) continue;

            foreach (Unit enemy in enemyList)
            {
                int distance = gridManager.GetHeuristic(gridManager.GetTile(enemy.gridPosition), tile);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    furthestTile = tile;
                }
            }
        }

        return furthestTile;
    }

    private void CheckBehaviour()
    {
        int unitsLeft = player.playerUnits.Count;

        if(unitsLeft > 25)
        {
            behaviour = AIBehaviours.Random;
        }
        else if (unitsLeft > 20)
        {
            behaviour = AIBehaviours.Dumb;
        }
        else if (unitsLeft > 15)
        {
            behaviour = AIBehaviours.Defensive;
        }
        else if (unitsLeft > 10)
        {
            behaviour = AIBehaviours.Aggressive;
        }
        else
        {
            behaviour = AIBehaviours.Smart;
        }
    }

    private void HandleUnitTurn()
    {
        if (activeUnit.isMoving) return;

        gridManager.GetHighlightRange(activeUnit.gridPosition, activeUnit.movementLeft, activeUnit.attackRange);

        Tile targetTile = null;

        switch (behaviour)
        {
            case AIBehaviours.Random:
                targetUnit = GetRandomTarget();                             /* DEPRACTED    targetTile = gridManager.GetClosestAttackTile(GetRandomTarget(), activeUnit); */
                break;
            case AIBehaviours.Dumb:
                if(Random.value > 0.5f)
                {
                    targetUnit = GetTargetByHealth(false);                  /* DEPRACTED    targetTile = gridManager.GetClosestAttackTile(GetTargetByHealth(false), activeUnit); */
                }
                else
                {
                    targetUnit = GetTargetByDistance(activeUnit, false);    /* DEPRACTED    targetTile = gridManager.GetClosestAttackTile(GetTargetByDistance(activeUnit, false), activeUnit); */
                }
                break;
            case AIBehaviours.Defensive:
                if (activeUnit.health < activeUnit.maxHealth * 0.3f)
                {
                    targetTile = GetFurthestTileFromEnemy();
                }
                else
                {
                    targetUnit = GetTargetByDistance(activeUnit);            /* DEPRACTED    targetTile = gridManager.GetClosestAttackTile(GetTargetByDistance(activeUnit), activeUnit); */
                }
                break;
            case AIBehaviours.Aggressive:
                targetUnit = GetTargetByDistance(activeUnit);                /* DEPRACTED    targetTile = gridManager.GetClosestAttackTile(GetTargetByDistance(activeUnit), activeUnit); */
                break;
            case AIBehaviours.Smart:
                targetUnit = GetTargetByHealth();                            /* DEPRACTED    targetTile = gridManager.GetClosestAttackTile(GetTargetByHealth(), activeUnit); */
                break;
        }

        if (targetUnit == null)
        {
            activeUnit.movementLeft = 0;
            activeUnit.attacksLeft = 0;
            return;
        }

        int distance = gridManager.GetHeuristic(gridManager.GetTile(activeUnit.gridPosition), gridManager.GetTile(targetUnit.gridPosition));

        if (distance <= activeUnit.attackRange && activeUnit.attacksLeft > 0)
        {
            activeUnit.Attack(targetUnit);
            return;
        }
        else
        {
            activeUnit.attacksLeft = 0;
        }

        if (targetTile == null)
        {
            targetTile = gridManager.GetClosestAttackTile(targetUnit, activeUnit);
        }

        if (targetTile != null)
        {
            List<Tile> path = gridManager.GetPath(gridManager.GetTile(activeUnit.gridPosition), targetTile);

            if (path != null && path.Count > 0)
            {
                activeUnit.MoveTo(path);
            }
            else
            {
                activeUnit.movementLeft = 0;
            }
        }
        else
        {
            activeUnit.movementLeft = 0;
        }

        if (activeUnit.IsUnitExpended())
        {
            EndActiveUnitTurn();
        }
    }

    private void EndActiveUnitTurn()
    {
        if (activeUnit != null)
        {
            units.Remove(activeUnit);
        }

        if (units.Count > 0)
        {
            activeUnit = units[0];
        }
        else
        {
            player.readyToEndTurn = true;
        }

        /*  REFACTORED
        gridManager.ResetGridHighlights();

        if (activeUnit.movementLeft <= 0 && activeUnit.attacksLeft <= 0)
        {
            targetUnit = null;
            units.Remove(activeUnit);

            if (units.Count > 0)
            {
                activeUnit = units[0];
            }
            else
            {
                player.readyToEndTurn = true;
            }
        }
        */
    }
}
