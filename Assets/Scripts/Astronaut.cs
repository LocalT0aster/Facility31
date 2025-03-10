using UnityEngine;

[RequireComponent(typeof(AstronautController))]
public class Astronaut : MonoBehaviour
{
    public float MaxHealth = 100f;
    public float Health = 100f;
    public float MaxCharge = 100f;
    public float Charge = 100f;
    public float FuelConsumption = 1f;

    private AstronautController ac;
    void Start() {
        ac = GetComponent<AstronautController>();
    }
    void FixedUpdate()
    {
        if (Charge <= 0f && Input.GetKeyDown(KeyCode.R)) {
            Charge = MaxCharge;
            ac.HasControl = true;
        }
    }
    public void ChargeDeplete(float cost) {
        Charge -= FuelConsumption * Time.fixedDeltaTime * cost;
        if (Charge < 0f) {
            ac.HasControl = false;
            Charge = 0f;
        }
    }
}
