using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Astronaut))]
[RequireComponent(typeof(CursorLock))]
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
        bool notDeadYet = astr.Health > 0f;
        HealthBar.fillAmount = notDeadYet ? astr.Health / astr.MaxHealth : 0f;
        
        if (notDeadYet) return;

        CursorLock cl = GetComponent<CursorLock>();
        cl.UpdateLock(false, false);
    }
}
