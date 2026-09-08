using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Map Settings")]
    public int width = 10;
    public int height = 10;
    public GameObject tilePrefab;

    [Header("Materials")]
    public Material lightMaterial;
    public Material darkMaterial;

    private Tile[,] map;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        map = new Tile[width, height];
        GenerateGrid();
    }

    //Generate the game grid
    public void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Spawn Tile Prefab and attach it to the GridManager
                Vector3 tilePosition = new Vector3(x - width / 2, 0, y - height / 2);   //Sets position at center
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);   //Creates Tile
                tile.name = $"Tile {x},{y}";
                tile.transform.SetParent(transform);

                Renderer renderer = tile.GetComponentInChildren<Renderer>();    //find and store the Renderer component in the Tile or its children

                renderer.material = new Material((x+y) % 2 == 0 ? lightMaterial : darkMaterial);    //Set tile color based on position in grid

                //Set tile position and add to map array. Assign default color
                Tile tileScript = tile.GetComponent<Tile>();
                tileScript.gridPosition = new Vector2Int(x, y);
                tileScript.originalColor = renderer.material.color;
                map[x, y] = tileScript;

                /* //Deprecated. Replaced with 1 line version above
                if ((x+y) % 2 == 0)
                {
                    renderer.material = new Material(lightMaterial);
                }
                else
                {
                    renderer.material = new Material(darkMaterial);
                }
                */
            }
        }
    }

    private List<Tile> GetTileNeighbors(Vector2Int tilePosition)
    {
        List<Tile> neighbors = new List<Tile>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int posX = tilePosition.x + x;
                int posY = tilePosition.y + y;

                if((posX >= 0) && (posY >= 0) && (posX < width) && (posY < height) && (new Vector2Int(posX, posY) != tilePosition)) //if neighbor is on the board and is NOT the current tile
                {
                    neighbors.Add(map[posX, posY]);
                }
            }
        }

        return neighbors;
    }

    private bool IsDiagonal(Tile a, Tile b)
    {
        int dx = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
        int dy = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);
        return dx == 1 && dy == 1;  //If the difference of both is 1, it's diagonal
    }

    private void ResetGridHighlights()
    {
        foreach (Tile tile in map)
        {
            if (Tile.selectedTile != tile)
            {
                tile.inMoveRange = false;
                tile.ChangeColor(tile.originalColor);
            }
        }
    }

    public List<Tile> GetHighlightRange(Vector2Int start, int range)
    {
        ResetGridHighlights();

        List<Tile> reachable = new List<Tile>();
        Dictionary<Tile, int> costSoFar = new Dictionary<Tile, int>();
        Queue<Tile> edge = new Queue<Tile>();

        Tile startTile = map[start.x, start.y];
        edge.Enqueue(startTile);
        costSoFar[startTile] = 0;

        while (edge.Count > 0)
        {
            Tile current = edge.Dequeue();
            int currentCost = costSoFar[current];

            foreach (Tile neighbor in GetTileNeighbors(current.gridPosition))
            {
                int stepCost = IsDiagonal(current, neighbor) ? 2 : 1;
                int newCost = currentCost + stepCost;

                if (newCost <= range && (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])) //check if new tile is in movement range and if the tile exists in dictionary already (or if the new cost is less than the previous)
                {
                    costSoFar[neighbor] = newCost;
                    edge.Enqueue(neighbor);

                    if (!reachable.Contains(neighbor) && neighbor != startTile)
                    {
                        reachable.Add(neighbor);
                        neighbor.inMoveRange = true;
                    }
                }
            }
        }

        return reachable;
    }

    public void HighlightMoveRange(Tile start, int range)
    {
        List<Tile> reachableTiles = GetHighlightRange(start.gridPosition, range);

        foreach (Tile tile in reachableTiles)
        {
            tile.ChangeColor(Color.cyan);
        }
    }

    public Tile GetTile(Vector2Int position)
    {
        return map[position.x, position.y];
    }
}
