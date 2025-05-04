using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody _rigidbody;
    public GameObject fixfoam;
    private bool triggered = false;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.velocity = transform.forward * speed;
    }
    public float damage = 10.0f;
    public int speed = 30;

    private void OnCollisionEnter(Collision other)
    {
        if (triggered)
        {
            return;
        }
        triggered = true;
        Debug.Log("fixfoam activated");
        GameObject foam;
        if (other.gameObject.CompareTag("Enemy"))
        {
            var otherObject = other.gameObject.GetComponent<EnemyFollow>();
            otherObject.StartCoroutine(otherObject.TemporaryDisable(10f));
            foam = Instantiate(fixfoam, other.gameObject.transform);
            foam.transform.position = transform.position;
            foam.transform.rotation = transform.rotation;
            Destroy(gameObject);
            return;
        }
        foam = Instantiate(fixfoam);
        foam.transform.position = transform.position;
        foam.transform.rotation = transform.rotation;
        Destroy(gameObject);
    }
}
