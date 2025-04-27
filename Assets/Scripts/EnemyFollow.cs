using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyFollow : MonoBehaviour {
    [Header("Target Settings")]
    [SerializeField] public Transform target;  // The point to follow
    private float stoppingDistance = 0.5f;
    public float StoppingDistance {
        get => stoppingDistance;
        set => stoppingDistance = Mathf.Max(0.1f, value);
    }
    public float NotifyProximity = 5f;
    private IProximityListener proximityListener;

    [Header("Movement Settings")]
    [SerializeField] private float forceMultiplier = 10f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float rotationSpeed = 2f;

    [Header("PID Controller")]
    [SerializeField] private float proportionalGain = 0.5f;  // P term
    [SerializeField] private float derivativeGain = 0.1f;    // D term

    private Rigidbody rb;
    private Vector3 lastError;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ConfigureRigidbody();
        if (TryGetComponent<IProximityListener>(out var proximityListener)) {
            this.proximityListener = proximityListener;
        } else {
            NotifyProximity = float.MinValue;
        }
    }

    void ConfigureRigidbody()
    {
        rb.useGravity = false;
        rb.drag = 0;
        rb.angularDrag = 0;
    }

    void FixedUpdate()
    {
        if (target == null) return;

        HandleRotation();
        HandleMovement();
    }

    void HandleRotation()
    {
        Vector3 directionToTarget = (target.position - rb.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation,
            rotationSpeed * Time.fixedDeltaTime));
    }

    void HandleMovement()
    {
        Vector3 error = target.position - rb.position;
        float distance = error.magnitude;

        if (distance < NotifyProximity) {
            proximityListener.OnProximity();
        }
        // Stop if within stopping distance
        if (distance < StoppingDistance)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        // Calculate PID forces
        Vector3 errorDerivative = (error - lastError) / Time.fixedDeltaTime;
        lastError = error;

        Vector3 acceleration = (proportionalGain * error) + (derivativeGain * errorDerivative);
        Vector3 force = Vector3.ClampMagnitude(acceleration * forceMultiplier, maxSpeed);

        // Apply force
        rb.AddForce(force, ForceMode.Acceleration);
    }
}
