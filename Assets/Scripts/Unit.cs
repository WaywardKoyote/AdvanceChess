using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Unit : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
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
    public Tile tileStartedTurnOn;

    public int maxHealth;
    public int health;
    public int attackDamage;

    public int attacksLeft = 1;

    public bool isAI;

    public int level = 1;
    public float experience = 0f;
    public float experienceToLevel = 100f;

    public float physicalDefense;

    public CanvasGroup healthBarVis;
    public Image healthBar;

    public UnitClass unitClass;
    public UnitStats characterGrowths;

    public string unitName;
    public Sprite unitPortrait;
    public Color unitColor;
    public Image spriteImage;

    private bool expendAfterMoving = false;

    private UIManager uiManager;
    private CameraController camControl;

    public ParticleSystem hitParticles;
    public AudioSource damageSFX;

    void Start()
    {
        InitializeStats();
        UpdateStatValues();
        movementLeft = movementRange;
        health = maxHealth;

        uiManager = FindAnyObjectByType<UIManager>();
        camControl = FindAnyObjectByType<CameraController>();
    }
    
    // Update is called once per frame
    void Update()
    {
        HandleMovement();

        healthBar.fillAmount = (float)health / (float)maxHealth;
    }

    public void MoveTo(List<Tile> newPath, bool expendAfterMove)
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

        if (expendAfterMove)
        {
            expendAfterMoving = true;
        }

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

            movementLeft -= path[0].moveCost;   // Is not accounting for diagonal move costs (diagonal move still only counts as 1)

            path.RemoveAt(0);

            if (path.Count > 0)
            {
                targetPosition = path[0].transform.position;
                gridPosition = path[0].gridPosition;
            }
            else
            {
                isMoving = false;

                if (expendAfterMoving)
                {
                    ExpendUnit();
                }

                if(!isAI && attacksLeft > 0) owner.ChangeSelectedUnit(this);
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
        if (owner.isPlayerTurn && !IsUnitExpended())
        {
            if (Player.selectedUnit != null && Player.selectedUnit.isMoving) return;

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

        Tile closestAttackTile = owner.gridManager.GetClosestAttackTile(this, playerUnit);

        if (distance <= playerUnit.attackRange && !this.owner.isPlayerTurn)
        {
            if (playerUnit.attacksLeft > 0)
            {
                playerUnit.Attack(this);
                return;
            }
            else
            {
                return;
            }
        }

        if (closestAttackTile == null)
        {
            return;
        }

        List<Tile> path = owner.gridManager.GetPath(attackerTile, closestAttackTile);
        
        if (path == null || path.Count == 0)
        {
            return;
        }

        if (distance <= (playerUnit.attackRange + playerUnit.movementLeft) && !this.owner.isPlayerTurn)
        {
            if (playerUnit.attacksLeft > 0)
            {
                playerUnit.MoveTo(path, false);
                StartCoroutine(DelayedAttack(playerUnit, this));
                return;
            }
            else
            {
                return;
            }
        }

        playerUnit.MoveTo(path, true);
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
            if(this.unitClass.name == "King")
            {
                if (this.isAI)
                {
                    uiManager.EndScreen(true);
                }
                else
                {
                    uiManager.EndScreen(false);
                }

                camControl.gameOver = true;
            }
            owner.gridManager.GetTile(gridPosition).isOccupied = false;
            owner.playerUnits.Remove(this);
            Destroy(gameObject);
        }
    }

    public void Attack(Unit target)
    {
        Tile attackerTile = owner.gridManager.GetTile(gridPosition);
        Tile targetTile = owner.gridManager.GetTile(target.gridPosition);

        int distance = owner.gridManager.GetHeuristic(attackerTile, targetTile);

        if (distance <= attackRange)
        {
            int damageToDeal;

            attacksLeft--;
            float attackDamage = CalculateAttDamage() * (1f - target.physicalDefense);       // int damageToDeal = Mathf.RoundToInt(Mathf.Clamp(CalculateAttDamage() * (1 - (target.physicalDefense)), 0, target.maxHealth));
            // float experienceToAdd = target.health <= damageToDeal ? attackDamage * 10 : attackDamage;
            // experienceToAdd *= Mathf.Clamp(10 - (level - target.level), 0, 10);

            Debug.Log("Def " + target.physicalDefense.ToString());
            Debug.Log(attackDamage.ToString());

            if (CritCheck())
            {
                damageToDeal = Mathf.RoundToInt(attackDamage * 1.5f);
                // Debug.Log("CRITICAL HIT");
            }
            else
            {
                damageToDeal = Mathf.RoundToInt(attackDamage);
            }

                // Debug.Log(this.name + " attacked " + target.name + " for " + damageToDeal.ToString() + " damage. " + target.name + " has " + (target.health - damageToDeal).ToString() + " health left.");
            
            if (damageToDeal < target.health && distance <= 1)
            {
                StartCoroutine(CounterAttack(1f, this, target));
            }

            hitParticles.transform.position = target.transform.position;
            hitParticles.Play();
            damageSFX.Play();

            target.TakeDamage(damageToDeal);

            if (attacksLeft <= 0) ExpendUnit();

            /* Leveling removed.
            experience += experienceToAdd;

            if (experience >= experienceToLevel)
            {
                LevelUp();
            }
            */
        }
    }

    public float CalculateAttDamage()
    {
        float damage = Random.Range((attackDamage * 0.95f), (attackDamage * 1.05f));
        damage = damage * ((float)health / (float)maxHealth);

        return damage;
    }

    public bool CritCheck()
    {
        bool critcalHit = false;

        switch (unitClass.name)
        {
            case "Pawn":
                {
                    int dx = Mathf.Abs(tileStartedTurnOn.gridPosition.x - gridPosition.x);
                    int dy = Mathf.Abs(tileStartedTurnOn.gridPosition.y - gridPosition.y);
                    if (dx == 1 && dy == 1) // If the difference of both is 1, it's diagonal
                    {
                        critcalHit = true;
                    }
                }
                break;
            case "Rook":
                {
                    int dx = Mathf.Abs(tileStartedTurnOn.gridPosition.x - gridPosition.x);
                    int dy = Mathf.Abs(tileStartedTurnOn.gridPosition.y - gridPosition.y);
                    if ((dx == 0 && dy == 3) || (dx == 3 && dy == 0)) // If the difference of either is 0, it's straight. If the difference of the other is 3, it's at max range
                    {
                        critcalHit = true;
                    }
                }
                break;
            case "Bishop":
                {
                    int dx = Mathf.Abs(tileStartedTurnOn.gridPosition.x - gridPosition.x);
                    int dy = Mathf.Abs(tileStartedTurnOn.gridPosition.y - gridPosition.y);
                    if (dx == 2 && dy == 2) // If the difference of both is 2, it's diagonal and at max range
                    {
                        critcalHit = true;
                    }
                }
                break;
            case "Knight":
                {
                    if (tileStartedTurnOn.gridPosition == gridPosition) // If the current gridPosition is the same as the starting gridPosition, the Unit didn't move
                    {
                        critcalHit = true;
                    }
                }
                break;
            case "Queen":
                {
                    if (health <= 40)
                    {
                        critcalHit = true;
                    }
                }
                break;
            case "King":
                {
                    if (health <= 60)
                    {
                        critcalHit = true;
                    }
                }
                break;
            default:
                Debug.Log("What class is this Unit?");
                break;
        }

        return critcalHit;
    }

    public bool IsUnitExpended()
    {
        if (movementLeft <= 0 && attacksLeft <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void UpdateStatValues()
    {
        movementRange = Mathf.RoundToInt(stats.moveRange);                                  // movementRange = Mathf.RoundToInt(stats.speed * 0.1f);
        attackRange = Mathf.RoundToInt(stats.attackRange);                                  // attackRange = Mathf.RoundToInt(stats.perception * 0.05f);
        attackRange = Mathf.Clamp(attackRange, 1, int.MaxValue);
        maxHealth = Mathf.Clamp(Mathf.RoundToInt(stats.health), 1, int.MaxValue);           // maxHealth = 1 + Mathf.RoundToInt(stats.endurance * 0.25f);
        attackDamage = Mathf.RoundToInt(stats.attackDamage);                                // attackDamage = Mathf.RoundToInt(stats.strength * 0.05f);
        physicalDefense = stats.defense * 0.01f;                          // physicalDefense = Mathf.RoundToInt((stats.health * 0.02f) + (stats.attackDamage * 0.01f));
    }

    private void LevelUp()
    {
        if (isAI) return;   // No levelling for AI

        experience -= experienceToLevel;
        experienceToLevel *= 1.5f;
        level++;

        stats.LevelUpStats(unitClass.classGrowths, characterGrowths);
        UpdateStatValues();

        if (experience >= experienceToLevel)
        {
            LevelUp();
        }
    }

    public void InitializeStats()
    {
        UnitStats baseClassStats = unitClass.baseStats;
        UnitStats growthStats = unitClass.classGrowths;

        stats = new UnitStats(
            UnitStats.InitializeStatValue(baseClassStats.moveRange, growthStats.moveRange),
            UnitStats.InitializeStatValue(baseClassStats.attackRange, growthStats.attackRange),
            UnitStats.InitializeStatValue(baseClassStats.health, growthStats.health),
            UnitStats.InitializeStatValue(baseClassStats.attackDamage, growthStats.attackDamage),
            UnitStats.InitializeStatValue(baseClassStats.defense, growthStats.defense));
            // UnitStats.InitializeStatValue(baseClassStats.intellect, growthStats.intellect),
            // UnitStats.InitializeStatValue(baseClassStats.spirit, growthStats.spirit),
            // UnitStats.InitializeStatValue(baseClassStats.mastery, growthStats.mastery));
    }

    public void ExpendUnit()
    {
        spriteImage.color = Color.gray7;
        movementLeft = 0;
        attacksLeft = 0;
        owner.DeselectUnit();
        owner.gridManager.ResetGridHighlights();
        owner.movesRemaining--;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Player.hoverUnit = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Player.hoverUnit == this)
        {
            Player.hoverUnit = null;
        }
    }

    // ---------- IEnumerators ----------

    IEnumerator DelayedAttack(Unit playerUnit, Unit targetUnit)
    {
        yield return new WaitUntil(() => !playerUnit.isMoving);

        playerUnit.Attack(targetUnit);
    }

    IEnumerator DelayedExpend(bool moving)
    {
        yield return new WaitUntil(() => !moving);

        ExpendUnit();
    }

    IEnumerator CounterAttack(float delay, Unit playerUnit, Unit targetUnit)
    {
        yield return new WaitForSeconds(delay);

        int damageToDeal = Mathf.RoundToInt(targetUnit.CalculateAttDamage() * (1 - playerUnit.physicalDefense));       // int damageToDeal = Mathf.RoundToInt(Mathf.Clamp(CalculateAttDamage() * (1 - (target.physicalDefense)), 0, target.maxHealth));
        // float experienceToAdd = target.health <= damageToDeal ? attackDamage * 10 : attackDamage;
        // experienceToAdd *= Mathf.Clamp(10 - (level - target.level), 0, 10);
        
        targetUnit.hitParticles.transform.position = playerUnit.transform.position;
        hitParticles.Play();
        damageSFX.Play();

        playerUnit.TakeDamage(damageToDeal);
    }
}
