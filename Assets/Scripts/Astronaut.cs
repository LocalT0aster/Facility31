using System;
using UnityEngine;

[RequireComponent(typeof(AstronautController))]
public class Astronaut : MonoBehaviour, IDamageble
{
    [Header("Health")]
    public float MaxHealth = 100f;
    public float Health = 100f;
    [Header("Charge")]
    public float MaxCharge = 100f;
    public float Charge = 100f;
    public float FuelConsumption = 1f;
    [Header("Damage")]
    public float DamageMultiplier = 1f;
    public float MinImpactSpeed = 10f;
    public float DamagePerSpeed = 5f;

    public GameObject DeathScreen;

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
                DeathScreen.SetActive(true);
            }
            OnDamaged?.Invoke(damage);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        ContactPoint[] contacts = new ContactPoint[collision.contactCount];
        collision.GetContacts(contacts);
        Vector3 CollisionCenter = Vector3.zero;
        float impactSpeed;

        if (contacts.Length > 0) {
            foreach (ContactPoint c in contacts) { // Calculate the Center of collision, to determine the actual velocity.
                CollisionCenter += c.point;
            }
            CollisionCenter /= contacts.Length;
            impactSpeed = Vector3.Project(collision.relativeVelocity, transform.position - CollisionCenter).magnitude;
        } else {
            Debug.LogWarning("We hit something, but without contact. Ignoring the collision.");
            return;
        }
        if (impactSpeed > MinImpactSpeed) {
            float damageAmount = (impactSpeed - MinImpactSpeed) * DamagePerSpeed;
            Damage(damageAmount);
        }
    }
}
