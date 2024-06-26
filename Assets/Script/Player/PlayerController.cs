using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float swipeThreshold = 50f;
    public float moveSpeed = 2f;

    private Vector2 startTouchPosition;
    private Vector2 currentTouchPosition;
    private Vector2 endTouchPosition;

    private Vector3 targetPosition;
    private bool isMoving = false;

    private GridManager gridManager;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("GridManager non trouvé dans la scène.");
        }

        targetPosition = transform.position;
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
        else
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    break;

                case TouchPhase.Moved:
                    currentTouchPosition = touch.position;
                    break;

                case TouchPhase.Ended:
                    endTouchPosition = touch.position;
                    Vector2 swipeDirection = endTouchPosition - startTouchPosition;

                    if (swipeDirection.magnitude > swipeThreshold)
                    {
                        swipeDirection.Normalize();
                        Vector3 direction = DetermineDirection(swipeDirection);

                        Vector3 potentialTargetPosition = CalculateTargetPosition(direction);
                        if (IsValidMove(potentialTargetPosition))
                        {
                            targetPosition = potentialTargetPosition;
                            isMoving = true;
                        }
                    }
                    break;
            }
        }
        else
        {
            HandleKeyboardInput();
        }
    }

    private void HandleKeyboardInput()
    {
        if (isMoving) return;

        Vector3 direction = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = Vector3.up;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = Vector3.down;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = Vector3.left;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = Vector3.right;
        }

        if (direction != Vector3.zero)
        {
            Vector3 potentialTargetPosition = CalculateTargetPosition(direction);
            if (IsValidMove(potentialTargetPosition))
            {
                targetPosition = potentialTargetPosition;
                isMoving = true;
            }
        }
    }

    private Vector3 DetermineDirection(Vector2 swipeDirection)
    {
        Vector3 direction = Vector3.zero;

        if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
        {
            direction = swipeDirection.x > 0 ? Vector3.right : Vector3.left;
        }
        else
        {
            direction = swipeDirection.y > 0 ? Vector3.up : Vector3.down;
        }

        return direction;
    }

    private Vector3 CalculateTargetPosition(Vector3 direction)
    {
        float tileSpacing = ScaleManager.Instance.GetTileSpacing();
        int gridSize = gridManager.GetGridSize();

        Vector3 newPosition = transform.position + direction * tileSpacing;

        float minPosition = -(gridSize - 1) / 2f * tileSpacing;
        float maxPosition = (gridSize - 1) / 2f * tileSpacing;

        newPosition.x = Mathf.Clamp(newPosition.x, minPosition, maxPosition);
        newPosition.y = Mathf.Clamp(newPosition.y, minPosition, maxPosition);

        return newPosition;
    }

    private bool IsValidMove(Vector3 position)
    {
        int gridSize = gridManager.GetGridSize();
        float tileSpacing = ScaleManager.Instance.GetTileSpacing();

        float minPosition = -(gridSize - 1) / 2f * tileSpacing;
        float maxPosition = (gridSize - 1) / 2f * tileSpacing;

        return position.x >= minPosition && position.x <= maxPosition &&
               position.y >= minPosition && position.y <= maxPosition;
    }
}
