using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileShooting : MonoBehaviour
{

    public Camera playerCamera;
    public GameObject bulletprefab;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Shoot();
        }
    }


    private void Shoot()
    {
        var proj = Instantiate(bulletprefab);
        proj.transform.position = playerCamera.transform.position + playerCamera.transform.forward * 3f;
        proj.transform.rotation = playerCamera.transform.rotation;
    }

}