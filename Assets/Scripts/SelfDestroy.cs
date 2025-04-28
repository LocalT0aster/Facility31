using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        StartCoroutine(CheckRoutine());

    }

    private IEnumerator CheckRoutine()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }

}
