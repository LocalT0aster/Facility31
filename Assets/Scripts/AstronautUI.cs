using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Astronaut))]
public class AstronautUI : MonoBehaviour
{
    public Image ChargeBar;
    public Image HealthBar;
    public Image HealthBarBg;
    private Astronaut astr;
    void Awake() {
        astr = GetComponent<Astronaut>();
        if (ChargeBar == null || HealthBar == null || HealthBarBg == null) throw new MissingReferenceException("No reference for Charge Bar");
        HealthBar.enabled = false;
        HealthBarBg.enabled = false;
    }

    void OnEnable() {
        astr.OnDamaged += DamageUpdate;
    }

    void OnDisable() {
        astr.OnDamaged -= DamageUpdate;
    }

    void Update() {
        ChargeBar.fillAmount = astr.Charge / astr.MaxCharge;
    }

    void DamageUpdate(float damage) {
        if (!HealthBar.enabled || !HealthBarBg.enabled) {
            HealthBar.enabled = true;
            HealthBarBg.enabled = true;
        }
        HealthBar.fillAmount = astr.Health > 0f ? astr.Health / astr.MaxHealth : 0f;
    }
}
