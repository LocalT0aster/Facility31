using UnityEngine;

public class PointerController : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public RectTransform safeZone;
    public float moveSpeed = 100f;
    public GameObject qteParent; // Parent object for all QTE elements

    [Header("Level Settings")]
    public int maxLevels = 3;
    public float speedMultiplierPerLevel = 1.5f;
    public float widthReductionPerLevel = 0.5f;
    [Range(0.1f, 0.9f)] public float positionChangeRange = 0.7f;

    private RectTransform pointerTransform;
    private Vector3 targetPosition;
    private int currentLevel = 1;
    private float initialMoveSpeed;
    private Vector2 initialSafeZoneSize;
    private Vector3 initialSafeZonePosition;
    private bool isActive = false;

    public Astronaut astr;

    void Start()
    {
        pointerTransform = GetComponent<RectTransform>();
        targetPosition = pointB.position;

        initialMoveSpeed = moveSpeed;
        initialSafeZoneSize = safeZone.sizeDelta;
        initialSafeZonePosition = safeZone.position;

        HideQTE(); // Start with QTE hidden
    }

    void Update()
    {
        if (astr.Health <= 0f) {
            HideQTE();
            this.enabled = false;
            return;
        }

        // Toggle QTE visibility with R key
        if (Input.GetKeyDown(KeyCode.R) && !isActive)
        {
            StartQTE();
        }

        if (!isActive) return;

        // Original movement code
        pointerTransform.position = Vector3.MoveTowards(pointerTransform.position, targetPosition,
                                                      moveSpeed * Time.deltaTime);

        if (Vector3.Distance(pointerTransform.position, pointA.position) < 0.1f)
        {
            targetPosition = pointB.position;
        }
        else if (Vector3.Distance(pointerTransform.position, pointB.position) < 0.1f)
        {
            targetPosition = pointA.position;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            CheckSuccess();
        }
    }

    void StartQTE()
    {
        isActive = true;
        qteParent.GetComponent<CanvasGroup>().alpha = 1;
        ResetGame();
        RepositionSafeZone();
    }

    void HideQTE()
    {
        isActive = false;
        qteParent.GetComponent<CanvasGroup>().alpha = 0;
    }

    void CheckSuccess()
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(safeZone, pointerTransform.position, null))
        {
            if (currentLevel < maxLevels)
            {
                currentLevel++;
                IncreaseDifficulty();
                Debug.Log($"Level {currentLevel} reached!");
                RepositionSafeZone();
            }
            else
            {
                Debug.Log("Final Success!");
                astr.Recharge();
                HideQTE();
            }
        }
        else
        {
            ResetGame();
            Debug.Log("Fail!");
        }
    }

    void IncreaseDifficulty()
    {
        moveSpeed = initialMoveSpeed * Mathf.Pow(speedMultiplierPerLevel, currentLevel - 1);
        float newWidth = initialSafeZoneSize.x * Mathf.Pow(widthReductionPerLevel, currentLevel - 1);
        safeZone.sizeDelta = new Vector2(newWidth, initialSafeZoneSize.y);
    }

    void RepositionSafeZone()
    {
        float leftBound = pointA.position.x + safeZone.rect.width / 2;
        float rightBound = pointB.position.x - safeZone.rect.width / 2;
        float availableRange = rightBound - leftBound;
        float newX = leftBound + availableRange * positionChangeRange * Random.Range(0.5f, 1.5f);
        newX = Mathf.Clamp(newX, leftBound, rightBound);
        safeZone.position = new Vector3(newX, initialSafeZonePosition.y, initialSafeZonePosition.z);
    }

    public void ResetGame()
    {
        currentLevel = 1;
        moveSpeed = initialMoveSpeed;
        safeZone.sizeDelta = initialSafeZoneSize;
        safeZone.position = initialSafeZonePosition;
        pointerTransform.position = pointA.position;
        targetPosition = pointB.position;
    }
}