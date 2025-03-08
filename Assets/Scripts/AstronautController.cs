using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AstronautController : MonoBehaviour {
    [Header("Movement Settings")]
    public float acceleration = 10f;    // Force for translation
    public float maxSpeed = 50f;        // Maximum speed (set high for "no limits")
    public float linearDamping = 1f;    // Coefficient for the extra damping force
    [Tooltip("Built-in drag value when linear damping is active")]
    public float builtinLinearDrag = 0.1f;
    [Tooltip("Velocity threshold below which the velocity is snapped to zero")]
    public float stopThreshold = 0.02f;

    [Header("Rotation Settings")]
    public float turnTorque = 100f;     // Torque factor for yaw (mouse horizontal)
    public float pitchTorque = 100f;    // Torque factor for pitch (mouse vertical)
    public float rollTorque = 100f;     // Torque factor for roll (Q/E keys)
    [Tooltip("Damping factor to reduce residual angular velocity when no input is given")]
    public float angularDamping = 5f;   // Damping factor for angular velocity

    // Toggles for damping
    private bool angularDampingEnabled = true;
    private bool linearDampingEnabled = true;

    private Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        // Set initial drag to zero, we'll override it as needed.
        rb.drag = 0f;
        // We'll handle angular damping manually, so set angularDrag to zero.
        rb.angularDrag = 0f;
    }

    // Check for toggle inputs in Update
    void Update() {
        if (Input.GetKeyDown(KeyCode.CapsLock)) {
            angularDampingEnabled = !angularDampingEnabled;
        }

        if (Input.GetKeyDown(KeyCode.Tab)) {
            linearDampingEnabled = !linearDampingEnabled;
        }
    }

    void FixedUpdate() {
        // ---- TRANSLATION INPUT ----
        float moveInput = Input.GetAxis("Vertical");     // Forward/backward
        float strafeInput = Input.GetAxis("Horizontal");   // Left/right

        // Vertical movement: Space for up, LeftControl for down
        float verticalInput = 0f;
        if (Input.GetKey(KeyCode.Space))
            verticalInput = 1f;
        else if (Input.GetKey(KeyCode.LeftControl))
            verticalInput = -1f;

        // Determine if any translation input exists
        bool translationInputActive = !Mathf.Approximately(moveInput, 0f) ||
                                      !Mathf.Approximately(strafeInput, 0f) ||
                                      !Mathf.Approximately(verticalInput, 0f);

        // ---- TRANSLATION FORCE ----
        Vector3 force = (transform.forward * moveInput +
                         transform.right * strafeInput +
                         transform.up * verticalInput) * acceleration;
        rb.AddForce(force);

        // Optional: Limit maximum speed (set maxSpeed high if you want "no limits")
        if (rb.velocity.magnitude > maxSpeed) {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        // ---- LINEAR DAMPING ----
        // If no translation input is provided and linear damping is enabled,
        // apply extra damping.
        if (linearDampingEnabled && !translationInputActive) {
            // Apply a damping force in addition to enabling built-in drag.
            Vector3 dampingForce = -rb.velocity.normalized * acceleration * linearDamping;
            rb.AddForce(dampingForce * Time.fixedDeltaTime, ForceMode.Acceleration);
            // Enable built-in drag for additional damping.
            rb.drag = builtinLinearDrag;
        } else {
            // Reset built-in drag when user provides input.
            rb.drag = 0f;
        }

        // ---- ROTATION VIA TORQUE ----
        float yawInput = Input.GetAxis("Mouse X");
        float pitchInput = -Input.GetAxis("Mouse Y");  // Inverted if needed

        float rollInput = 0f;
        if (Input.GetKey(KeyCode.Q))
            rollInput = 1f;
        if (Input.GetKey(KeyCode.E))
            rollInput = -1f;

        Vector3 torque = Vector3.zero;
        torque += transform.up * yawInput * turnTorque;
        torque += transform.right * pitchInput * pitchTorque;
        torque += transform.forward * rollInput * rollTorque;

        rb.AddTorque(torque * Time.fixedDeltaTime, ForceMode.Acceleration);

        // ---- ANGULAR DAMPING ----
        if (angularDampingEnabled &&
            Mathf.Approximately(yawInput, 0f) &&
            Mathf.Approximately(pitchInput, 0f) &&
            Mathf.Approximately(rollInput, 0f)) {
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, angularDamping * Time.fixedDeltaTime);
        }
    }
}
