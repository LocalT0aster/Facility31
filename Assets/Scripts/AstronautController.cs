using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Astronaut))]
public class AstronautController : MonoBehaviour {
    [Header("Movement Settings")]
    public float acceleration = 200f;    // Force for translation
    public float maxSpeed = 1000f;        // Maximum speed (set high for "no limits")
    public float linearDamping = 1f;    // Coefficient for the extra damping force
    [Tooltip("Built-in drag value when linear damping is active")]
    public float builtinLinearDrag = 0.1f;
    [Tooltip("Velocity threshold below which the velocity is snapped to zero")]
    public float stopThreshold = 0.02f;

    [Header("Rotation Settings")]
    public float turnTorque = 500f;     // Torque factor for yaw (mouse horizontal)
    public float pitchTorque = 500f;    // Torque factor for pitch (mouse vertical)
    public float rollTorque = 500f;     // Torque factor for roll (Q/E keys)
    [Tooltip("Damping factor to reduce residual angular velocity when no input is given")]
    public float angularDamping = 1.7f;   // Damping factor for angular velocity

    // Toggles for damping
    private bool angularDampingEnabled = true;
    private bool linearDampingEnabled = true;

    public bool HasControl = true;

    private Rigidbody rb;
    private Astronaut astr;
    private float currentCost;

    // Axis
    private const string InputVerticalAxis = "Vertical";
    private const string InputHorizontalAxis = "Horizontal";
    private const string InputMouseXAxis = "Mouse X";
    private const string InputMouseYAxis = "Mouse Y";

    void Start() {
        astr = GetComponent<Astronaut>();
        rb = GetComponent<Rigidbody>();
        //rb.useGravity = false;
        // Set initial drag to zero, we'll override it as needed.
        rb.drag = 0f;
        // We'll handle angular damping manually, so set angularDrag to zero.
        rb.angularDrag = 0f;
    }

    void ToggleControl() {
        HasControl = !HasControl;
    }

    // Check for toggle inputs in Update
    void Update() {
        if (HasControl) {
            if (Input.GetKeyDown(KeyCode.CapsLock)) {
                angularDampingEnabled = !angularDampingEnabled;
            }
            if (Input.GetKeyDown(KeyCode.Tab)) {
                linearDampingEnabled = !linearDampingEnabled;
            }
        }
    }

    void FixedUpdate() {
        if (!HasControl) return;
        currentCost = 0f;

        // ---- TRANSLATION INPUT ----
        float moveInput = Input.GetAxis(InputVerticalAxis);     // Forward/backward
        float strafeInput = Input.GetAxis(InputHorizontalAxis);   // Left/right

        // Vertical movement: Space for up, LeftControl for down
        float verticalInput = 0f;
        if (Input.GetKey(KeyCode.Space))
            verticalInput = 1f;
        else if (Input.GetKey(KeyCode.LeftControl))
            verticalInput = -1f;

        currentCost += Mathf.Abs(moveInput) + Mathf.Abs(strafeInput) + Mathf.Abs(verticalInput);

        // Determine if any translation input exists
        bool translationInputActive = !Mathf.Approximately(moveInput, 0f) ||
                                      !Mathf.Approximately(strafeInput, 0f) ||
                                      !Mathf.Approximately(verticalInput, 0f);

        // ---- TRANSLATION FORCE ----
        Vector3 force = (transform.forward * moveInput +
                         transform.right * strafeInput +
                         transform.up * verticalInput) * acceleration;
        rb.AddForce(force);

        // Clamp max speed
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
            if (rb.velocity.magnitude >= 0.1f) {
                currentCost += 1f;
            }
        } else {
            // Reset built-in drag when user provides input.
            rb.drag = 0f;
        }

        // ---- ROTATION VIA TORQUE ----
        float yawInput = Input.GetAxis(InputMouseXAxis);
        float pitchInput = -Input.GetAxis(InputMouseYAxis);

        float rollInput = 0f;
        if (Input.GetKey(KeyCode.Q))
            rollInput = 1f;
        if (Input.GetKey(KeyCode.E))
            rollInput = -1f;

        currentCost += Mathf.Abs(yawInput) + Mathf.Abs(pitchInput) + Mathf.Abs(rollInput);

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
            if (rb.angularVelocity.magnitude >= 0.05f) {
                currentCost += 1f;
            }
        }

        astr.ChargeDeplete(currentCost);
    }
}
