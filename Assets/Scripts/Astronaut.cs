using System;
using UnityEngine;

[RequireComponent(typeof(AstronautController))]
public class Astronaut : MonoBehaviour, IDamageble
{
    public float MaxHealth = 100f;
    public float Health = 100f;
    public float DamageMultiplier = 1f;
    public float MaxCharge = 100f;
    public float Charge = 100f;
    public float FuelConsumption = 1f;

    private AstronautController ac;
    public event Action<float> OnDamaged;

    void Start() {
        ac = GetComponent<AstronautController>();
    }
  
    public void ChargeDeplete(float cost) {
        Charge -= FuelConsumption * Time.fixedDeltaTime * cost;
        if (Charge < 0f) {
            ac.HasControl = false;
            Charge = 0f;
        }
    }

    public void Recharge()
    {
        if (Health > 0f) {
            Charge = MaxCharge;
            ac.HasControl = true;
        }
    }
    public void Damage(float damage) {
        if (Health > 0f) {
            Health -= damage * DamageMultiplier;
            if (Health <= 0f) {
                ac.enabled = false;
                Charge = 0f;
            }
            OnDamaged?.Invoke(damage);
        }
    }
}
