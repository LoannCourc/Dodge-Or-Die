using UnityEngine;
using DG.Tweening; // Ajouter cette ligne pour utiliser DoTween

public class PlayerController : MonoBehaviour
{
    public float swipeThreshold = 50f;
    public float moveSpeed = 2f;
    public string[] swipeSoundNames; // Tableau de noms de sons de swipe
    public float stretchDuration = 0.2f; // Durée du stretch
    public float stretchAmount = 1.2f; // Facteur de stretch

    private Vector2 startTouchPosition;
    private Vector2 currentTouchPosition;
    private Vector2 endTouchPosition;

    private Vector3 targetPosition;
    private bool isMoving = false;

    private GridManager gridManager;
    private Vector3 initialScale; // Variable pour stocker l'échelle initiale

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("GridManager non trouvé dans la scène.");
        }

        targetPosition = transform.position;
        initialScale = transform.localScale; // Stocker l'échelle initiale
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

                // Revenir à l'échelle initiale après le mouvement
                transform.DOScale(initialScale, stretchDuration).SetEase(Ease.OutBack);
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

                            // Jouer un son aléatoire de swipe seulement si le mouvement est valide
                            PlayRandomSwipeSound();

                            // Appliquer le stretch dans la direction du swipe
                            ApplyStretch(direction);
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
             
                // Jouer un son aléatoire de swipe seulement si le mouvement est valide
                PlayRandomSwipeSound();

                // Appliquer le stretch dans la direction du swipe
                ApplyStretch(direction);
            }
        }
    }

    // Fonction pour vérifier si un mouvement est valide
    private bool IsValidMove(Vector3 position)
    {
        int gridSize = gridManager.GetGridSize();
        float tileSpacing = ScaleManager.Instance.GetTileSpacing();

        float minPosition = -(gridSize - 1) / 2f * tileSpacing;
        float maxPosition = (gridSize - 1) / 2f * tileSpacing;

        return position.x >= minPosition && position.x <= maxPosition &&
               position.y >= minPosition && position.y <= maxPosition;
    }

    // Fonction pour jouer un son de swipe aléatoire
    private void PlayRandomSwipeSound()
    {
        if (swipeSoundNames.Length == 0)
        {
            Debug.LogWarning("No swipe sound names defined.");
            return;
        }

        string randomSoundName = swipeSoundNames[Random.Range(0, swipeSoundNames.Length)];
        AudioManager.Instance.PlaySound(randomSoundName);
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

    // Fonction pour appliquer le stretch dans la direction du mouvement
    private void ApplyStretch(Vector3 direction)
    {
        Vector3 stretchVector = initialScale; // Utiliser l'échelle initiale comme base
        if (direction == Vector3.up || direction == Vector3.down)
        {
            stretchVector.y *= stretchAmount;
        }
        else if (direction == Vector3.left || direction == Vector3.right)
        {
            stretchVector.x *= stretchAmount;
        }

        transform.DOScale(stretchVector, stretchDuration).SetEase(Ease.OutQuad);
    }
}
