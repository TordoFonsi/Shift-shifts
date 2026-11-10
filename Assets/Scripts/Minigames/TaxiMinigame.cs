using System.Collections.Generic;
using UnityEngine;

public class TaxiMinigame : MonoBehaviour
{
    private enum CellType
    {
        Empty,
        Taxi,
        Destination,
        Obstacle
    }

    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private JobStability jobStability;
    [SerializeField] private JobSwitcher jobSwitcher;
    [SerializeField] private Transform gridContainer;
    [SerializeField] private TaxiCell cellPrefab;

    [Header("Grid Settings")]
    [SerializeField] private int gridSize = 5;
    [SerializeField] private int obstacleCount = 5;

    [Header("Colors")]
    [SerializeField]
    private Color emptyColor =
        new Color(0.20f, 0.23f, 0.28f);

    [SerializeField]
    private Color taxiColor =
        new Color(0.20f, 0.55f, 0.95f);

    [SerializeField]
    private Color destinationColor =
        new Color(0.95f, 0.75f, 0.15f);

    [SerializeField]
    private Color obstacleColor =
        new Color(0.75f, 0.20f, 0.20f);

    private CellType[,] grid;

    private readonly List<TaxiCell> cells =
        new List<TaxiCell>();

    private Vector2Int taxiPosition;
    private Vector2Int destinationPosition;

    private void Update()
    {
        if (!gameManager.IsPlaying) return;
        if (!jobSwitcher.IsTaxiActive) return;

        if (Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveTaxi(Vector2Int.down);
        }

        if (Input.GetKeyDown(KeyCode.S) ||
            Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveTaxi(Vector2Int.up);
        }

        if (Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveTaxi(Vector2Int.left);
        }

        if (Input.GetKeyDown(KeyCode.D) ||
            Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveTaxi(Vector2Int.right);
        }
    }

    public void ResetMinigame()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        ClearGrid();

        grid = new CellType[gridSize, gridSize];

        taxiPosition = Vector2Int.zero;

        destinationPosition =
            new Vector2Int(gridSize - 1, gridSize - 1);

        grid[taxiPosition.x, taxiPosition.y] =
            CellType.Taxi;

        grid[destinationPosition.x, destinationPosition.y] =
            CellType.Destination;

        GenerateObstacles();

        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                int index = y * gridSize + x;

                TaxiCell cell = Instantiate(
                    cellPrefab,
                    gridContainer
                );

                cell.Setup(this, index);
                cells.Add(cell);
            }
        }

        UpdateVisuals();
    }

    private void GenerateObstacles()
    {
        List<Vector2Int> candidates =
            new List<Vector2Int>();

        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                Vector2Int position =
                    new Vector2Int(x, y);

                if (position == taxiPosition ||
                    position == destinationPosition)
                {
                    continue;
                }

                candidates.Add(position);
            }
        }

        Shuffle(candidates);

        int placed = 0;

        foreach (Vector2Int position in candidates)
        {
            if (placed >= obstacleCount)
                break;

            grid[position.x, position.y] =
                CellType.Obstacle;

            if (HasValidPath())
            {
                placed++;
            }
            else
            {
                grid[position.x, position.y] =
                    CellType.Empty;
            }
        }
    }

    private bool HasValidPath()
    {
        Queue<Vector2Int> queue =
            new Queue<Vector2Int>();

        HashSet<Vector2Int> visited =
            new HashSet<Vector2Int>();

        queue.Enqueue(taxiPosition);
        visited.Add(taxiPosition);

        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == destinationPosition)
                return true;

            foreach (Vector2Int direction in directions)
            {
                Vector2Int next = current + direction;

                if (!IsInsideGrid(next))
                    continue;

                if (visited.Contains(next))
                    continue;

                if (grid[next.x, next.y] ==
                    CellType.Obstacle)
                {
                    continue;
                }

                visited.Add(next);
                queue.Enqueue(next);
            }
        }

        return false;
    }

    private void MoveTaxi(Vector2Int direction)
    {
        Vector2Int target = taxiPosition + direction;

        if (!IsInsideGrid(target))
            return;

        CellType targetType =
            grid[target.x, target.y];

        if (targetType == CellType.Obstacle)
        {
            jobStability.PenalizeTaxi();
            return;
        }

        grid[taxiPosition.x, taxiPosition.y] =
            CellType.Empty;

        taxiPosition = target;

        if (targetType == CellType.Destination)
        {
            jobStability.CompleteTaxiTask();
            GenerateGrid();
            return;
        }

        grid[taxiPosition.x, taxiPosition.y] =
            CellType.Taxi;

        UpdateVisuals();
    }

    public void SelectCell(int index)
    {
        if (!gameManager.IsPlaying ||
            !jobSwitcher.IsTaxiActive)
        {
            return;
        }

        int x = index % gridSize;
        int y = index / gridSize;

        Vector2Int target =
            new Vector2Int(x, y);

        int distance =
            Mathf.Abs(target.x - taxiPosition.x) +
            Mathf.Abs(target.y - taxiPosition.y);

        if (distance != 1)
            return;

        Vector2Int direction =
            target - taxiPosition;

        MoveTaxi(direction);
    }

    private bool IsInsideGrid(Vector2Int position)
    {
        return position.x >= 0 &&
               position.x < gridSize &&
               position.y >= 0 &&
               position.y < gridSize;
    }

    private void UpdateVisuals()
    {
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                int index = y * gridSize + x;

                CellType type = grid[x, y];

                Color color = emptyColor;
                string symbol = "";

                switch (type)
                {
                    case CellType.Taxi:
                        color = taxiColor;
                        symbol = "TAXI";
                        break;

                    case CellType.Destination:
                        color = destinationColor;
                        symbol = "GOAL";
                        break;

                    case CellType.Obstacle:
                        color = obstacleColor;
                        symbol = "X";
                        break;
                }

                cells[index].SetCell(color, symbol);
            }
        }
    }

    private void ClearGrid()
    {
        cells.Clear();

        for (int i = gridContainer.childCount - 1;
             i >= 0;
             i--)
        {
            Destroy(gridContainer.GetChild(i).gameObject);
        }
    }

    private void Shuffle(List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex =
                Random.Range(i, list.Count);

            (list[i], list[randomIndex]) =
                (list[randomIndex], list[i]);
        }
    }
}
