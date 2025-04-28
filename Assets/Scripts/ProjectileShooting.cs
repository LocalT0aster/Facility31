using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileShooting : MonoBehaviour
{

    public Camera playerCamera;
    public GameObject bulletprefab;

    public float fuel_consuption = 10.0f;

    private Astronaut astr;

    void Start()
    {
        astr = GetComponent<Astronaut>();
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Shoot();
        }
    }


    private void Shoot()
    {
        if (astr.Charge < fuel_consuption) { return; }
        var proj = Instantiate(bulletprefab);
        proj.transform.position = playerCamera.transform.position + playerCamera.transform.forward * 3f;
        proj.transform.rotation = playerCamera.transform.rotation;
        astr.Charge -= fuel_consuption;
        astr.ChargeDeplete(0);
    }

}