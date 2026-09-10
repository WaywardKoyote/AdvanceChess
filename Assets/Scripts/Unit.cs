using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Unit : MonoBehaviour, IPointerDownHandler
{
    public Vector2Int gridPosition;
    public float moveSpeed = 5f;
    private bool isMoving = false;
    private Vector3 targetPosition;

    private const float STOPPING_DISTANCE = 0.01f;

    public UnitStats stats;

    public int movementRange = 3;
    public int attackRange = 1;

    public Player owner;

    public List<Tile> path;

    void Start()
    {
        stats = new UnitStats(Random.Range(0, 100), Random.Range(0, 100));
        movementRange = Mathf.RoundToInt(stats.speed * 0.1f);
        attackRange = Mathf.RoundToInt(stats.perception * 0.05f);
        attackRange = Mathf.Clamp(attackRange, 1, int.MaxValue);
    }
    
    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    public void MoveTo(Vector3 position, Vector2Int gridPos)
    {
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

            path.RemoveAt(0);

            if (path.Count > 0)
            {
                MoveTo(path[0].transform.position, path[0].gridPosition);
            }
        }
    }

    public void OnPointerDown (PointerEventData eventData)
    {
        owner.ChangeSelectedUnit(this);
    }
}
