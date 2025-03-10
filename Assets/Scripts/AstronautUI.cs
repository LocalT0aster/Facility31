using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Astronaut))]
public class AstronautUI : MonoBehaviour
{
    public Image ChargeBar;
    private Astronaut astr;
    void Start() {
        astr = GetComponent<Astronaut>();
        if (ChargeBar == null) throw new MissingReferenceException("No reference for Charge Bar");
    }

    void Update() {
        ChargeBar.fillAmount = astr.Charge / astr.MaxCharge;
    }
}
