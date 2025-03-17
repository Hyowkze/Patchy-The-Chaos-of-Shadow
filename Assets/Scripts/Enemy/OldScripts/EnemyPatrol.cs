using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyPatrol : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float minHeightDifference = 2.5f;
    
    [Header("Jump Physics")]
    [SerializeField] private float maxJumpHeight = 7f;
    [SerializeField] private float jumpForwardForce = 2.2f;
    [SerializeField] private float jumpCooldown = 0.4f;
    
    [Header("Drop Settings")]
    [SerializeField] private float dropCheckDistance = 3f;
    [SerializeField] private float dropForwardForce = 0.5f;
    
    [Header("Detection Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask jumpZoneLayer;
    [SerializeField] private LayerMask dropZoneLayer;
    [SerializeField] private float jumpZoneRadius = 3f;
    
    [Header("A* Pathfinding")]
    [SerializeField] private Vector2 gridWorldSize = new Vector2(10, 10);
    [SerializeField] private float nodeRadius = 0.5f;
    [SerializeField] private float pathUpdateMinDistance = 0.5f;
    
    [Header("Transition Settings")]
    [SerializeField] private LayerMask jumpPointsLayer;
    [SerializeField] private LayerMask dropPointsLayer;
    [SerializeField] private float transitionDetectionRadius = 0.5f;
    
    [Header("Jump/Drop Forces")]
    [SerializeField] private float verticalJumpForce = 12f;
    [SerializeField] private float verticalDropForce = 5f;
    [SerializeField] private float horizontalTransitionForce = 2f;

    [Header("Collider References")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float groundCheckRadius = 0.4f;
    [SerializeField] private float wallCheckDistance = 1.2f;

    [Header("Pathfinding")]
    [SerializeField] private float pathUpdateRate = 0.3f;
    [SerializeField] private float nodeDistance = 0.5f;

    private Rigidbody2D rb;
    private Transform player;
    private bool isGrounded;
    private bool isFacingRight = true;
    private Vector2 movement;
    private GridAStar grid;
    private List<Node> currentPath = new List<Node>();
    private List<Vector2> platformTransitions = new List<Vector2>();
    private int targetPathIndex;
    private float pathUpdateTimer;
    private float jumpForce;
    private float nextJumpTime;
    private bool inJumpZone;
    private Vector3 playerPreviousPosition;
    private bool playerMovingUp;
    private bool shouldDrop;
    private Collider2D currentTransitionPoint;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerPreviousPosition = player.position;
        grid = new GridAStar(gridWorldSize, nodeRadius, groundLayer, wallLayer);
        CalculateJumpForce();
    }

    private void CalculateJumpForce()
    {
        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        jumpForce = Mathf.Sqrt(2 * maxJumpHeight * gravity);
    }

    private void Update()
    {
        if (player == null) return;

        TrackPlayerMovement();
        UpdateGroundDetection();
        UpdateJumpZoneDetection();
        UpdateTransitionPoints();
        UpdatePathfinding();
        HandleMovement();
        VisualDebugging();
    }

    private void TrackPlayerMovement()
    {
        float verticalMovement = player.position.y - playerPreviousPosition.y;
        playerMovingUp = verticalMovement > 0.05f;
        shouldDrop = verticalMovement < -0.05f;
        playerPreviousPosition = player.position;
    }

    private void UpdateGroundDetection()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void UpdateJumpZoneDetection()
    {
        Collider2D[] detectedJumpZones = Physics2D.OverlapCircleAll(transform.position, jumpZoneRadius, jumpZoneLayer);
        Collider2D[] detectedDropZones = Physics2D.OverlapCircleAll(transform.position, dropCheckDistance, dropZoneLayer);
        
        inJumpZone = false;
        shouldDrop = false;

        foreach (Collider2D zone in detectedJumpZones)
        {
            if (player.position.y > transform.position.y + minHeightDifference)
            {
                inJumpZone = true;
                break;
            }
        }

        foreach (Collider2D zone in detectedDropZones)
        {
            if (player.position.y < transform.position.y - minHeightDifference)
            {
                shouldDrop = true;
                break;
            }
        }
    }

    private void UpdateTransitionPoints()
    {
        platformTransitions.Clear();
        Collider2D[] nearbyPoints = Physics2D.OverlapCircleAll(
            transform.position, 
            transitionDetectionRadius,
            jumpPointsLayer | dropPointsLayer
        );

        foreach (Collider2D point in nearbyPoints)
        {
            platformTransitions.Add(point.transform.position);
        }
    }

    private void UpdatePathfinding()
    {
        pathUpdateTimer -= Time.deltaTime;
        if (pathUpdateTimer <= 0 || !isGrounded || ShouldUpdatePath())
        {
            grid.CreateGrid(transform.position);
            currentPath = AStar.FindPath(grid, transform.position, player.position);
            targetPathIndex = 0;
            pathUpdateTimer = pathUpdateRate;
        }
    }

    private bool ShouldUpdatePath()
    {
        return Vector2.Distance(player.position, transform.position) > pathUpdateMinDistance ||
               (currentPath != null && currentPath.Count > 0 && 
                Vector2.Distance(currentPath.Last().worldPosition, player.position) > nodeDistance);
    }

    private void HandleMovement()
    {
        if (currentPath == null || currentPath.Count == 0) return;

        Vector2 currentWaypoint = currentPath[targetPathIndex].worldPosition;
        Vector2 direction = (currentWaypoint - (Vector2)transform.position).normalized;
        movement.x = direction.x;

        if (Vector2.Distance(transform.position, currentWaypoint) < nodeDistance)
        {
            targetPathIndex++;
            if (targetPathIndex >= currentPath.Count) currentPath.Clear();
        }

        HandleJumpLogic(direction);
        HandleFlipping(direction);
    }

    private void HandleJumpLogic(Vector2 direction)
    {
        if (isGrounded && Time.time >= nextJumpTime && ShouldJump())
        {
            ExecuteJump(direction);
        }
    }

    private bool ShouldJump()
    {
        bool heightCondition = playerMovingUp ? 
            player.position.y > transform.position.y + minHeightDifference : 
            player.position.y < transform.position.y - minHeightDifference;

        bool wallDetected = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, groundLayer);
        
        return (heightCondition && !Physics2D.Linecast(transform.position, player.position, groundLayer)) || wallDetected || inJumpZone;
    }

    private void ExecuteJump(Vector2 direction)
    {
        float horizontalForce = Mathf.Clamp(Mathf.Abs(player.position.x - transform.position.x), 0.5f, 3f);
        float verticalMultiplier = playerMovingUp ? 1f : 0.7f;
        float horizontalMultiplier = playerMovingUp ? jumpForwardForce : dropForwardForce;

        rb.linearVelocity = new Vector2(
            direction.x * horizontalForce * horizontalMultiplier,
            jumpForce * verticalMultiplier
        );
        nextJumpTime = Time.time + jumpCooldown;
    }

    private void ExecuteTransition(Vector2 direction)
    {
        if (currentTransitionPoint == null) return;

        bool isJumpPoint = currentTransitionPoint.gameObject.layer == 
                          LayerMask.NameToLayer("JumpPoints");
        
        float verticalForce = isJumpPoint ? verticalJumpForce : -verticalDropForce;

        rb.linearVelocity = new Vector2(
            direction.x * horizontalTransitionForce,
            verticalForce
        );
    }

    private void HandleFlipping(Vector2 direction)
    {
        if ((direction.x > 0 && !isFacingRight) || (direction.x < 0 && isFacingRight))
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);
    }

    private void VisualDebugging()
    {
        Debug.DrawRay(transform.position, Vector2.up * maxJumpHeight, playerMovingUp ? Color.cyan : Color.yellow);
        Debug.DrawRay(wallCheck.position, transform.right * wallCheckDistance, Color.red);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & jumpZoneLayer) != 0)
        {
            UpdatePathToPlayerImmediate();
        }
        else if (((1 << other.gameObject.layer) & dropZoneLayer) != 0)
        {
            shouldDrop = true;
            UpdatePathToPlayerImmediate();
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("JumpPoints") || 
               other.gameObject.layer == LayerMask.NameToLayer("DropPoints"))
        {
            currentTransitionPoint = other;
            ExecuteTransition(GetDirectionToPlayer());
        }
    }

    private Vector2 GetDirectionToPlayer()
    {
        return (player.position - transform.position).normalized;
    }

    private void UpdatePathToPlayerImmediate()
    {
        grid.CreateGrid(transform.position);
        currentPath = AStar.FindPath(grid, transform.position, player.position);
        targetPathIndex = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, jumpZoneRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dropCheckDistance);

        if (grid != null && grid.grid != null)
        {
            foreach (Node n in grid.grid)
            {
                Gizmos.color = n.walkable ? Color.green : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (grid.NodeRadius * 2 - 0.1f));
            }

            if (currentPath != null)
            {
                Gizmos.color = Color.magenta;
                foreach (Node node in currentPath)
                {
                    Gizmos.DrawSphere(node.worldPosition, grid.NodeRadius);
                }
            }
        }
    }

    public class Node
    {
        public bool walkable;
        public Vector2 worldPosition;
        public int gridX;
        public int gridY;
        public int gCost;
        public int hCost;
        public Node parent;

        public int fCost => gCost + hCost;

        public Node(bool walkable, Vector2 worldPos, int gridX, int gridY)
        {
            this.walkable = walkable;
            this.worldPosition = worldPos;
            this.gridX = gridX;
            this.gridY = gridY;
        }
    }

    public class GridAStar
    {
        public Node[,] grid;
        private Vector2 gridBottomLeft;
        public float NodeRadius { get; private set; }
        private LayerMask groundMask;
        private LayerMask wallMask;
        private float nodeDiameter;
        public int GridSizeX, GridSizeY;

        public GridAStar(Vector2 worldSize, float nodeRadius, LayerMask groundMask, LayerMask wallMask)
        {
            this.NodeRadius = nodeRadius;
            this.groundMask = groundMask;
            this.wallMask = wallMask;
            nodeDiameter = nodeRadius * 2;
            GridSizeX = Mathf.RoundToInt(worldSize.x / nodeDiameter);
            GridSizeY = Mathf.RoundToInt(worldSize.y / nodeDiameter);
        }

        public void CreateGrid(Vector2 center)
        {
            grid = new Node[GridSizeX, GridSizeY];
            gridBottomLeft = center - new Vector2(GridSizeX / 2f, GridSizeY / 2f) * nodeDiameter;

            for (int x = 0; x < GridSizeX; x++)
            {
                for (int y = 0; y < GridSizeY; y++)
                {
                    Vector2 worldPoint = gridBottomLeft + 
                        new Vector2(x * nodeDiameter + NodeRadius, 
                                    y * nodeDiameter + NodeRadius);
                    
                    bool groundCollision = Physics2D.OverlapCircle(worldPoint, NodeRadius, groundMask);
                    bool wallCollision = Physics2D.OverlapCircle(worldPoint, NodeRadius, wallMask);
                    
                    grid[x, y] = new Node(!(groundCollision || wallCollision), worldPoint, x, y);
                }
            }
        }

        public Node NodeFromWorldPoint(Vector2 worldPosition)
        {
            float percentX = (worldPosition.x - gridBottomLeft.x) / (GridSizeX * nodeDiameter);
            float percentY = (worldPosition.y - gridBottomLeft.y) / (GridSizeY * nodeDiameter);
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((GridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((GridSizeY - 1) * percentY);
            return grid[x, y];
        }
    }

    public static class AStar
    {
        public static List<Node> FindPath(GridAStar grid, Vector2 startPos, Vector2 targetPos)
        {
            Node startNode = grid.NodeFromWorldPoint(startPos);
            Node targetNode = grid.NodeFromWorldPoint(targetPos);

            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost || 
                        (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                foreach (Node neighbour in GetNeighbours(grid, currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
            return null;
        }

        private static List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            path.Reverse();
            return path;
        }

        private static int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
            return dstX + dstY;
        }

        private static List<Node> GetNeighbours(GridAStar grid, Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < grid.GridSizeX && checkY >= 0 && checkY < grid.GridSizeY)
                    {
                        neighbours.Add(grid.grid[checkX, checkY]);
                    }
                }
            }
            return neighbours;
        }
    }
}