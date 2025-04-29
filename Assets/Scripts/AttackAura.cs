using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class AttackAura : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private Astronaut target;

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            target.Damage(50f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            target = other.GetComponent<Astronaut>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            target = null;
        }
    }
}
