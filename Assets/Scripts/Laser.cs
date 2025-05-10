using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.Events;

public class Laser : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float laserDistance = 8f;
    [SerializeField] private LayerMask ignoreMask;
    [SerializeField] private UnityEvent OnHitTarget;

    public GameObject LaserSound;

    private RaycastHit rayHit;
    private Ray ray;

    private void Awake()
    {
        lineRenderer.positionCount = 2;
    }

    private void FixedUpdate()
    {
        ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out rayHit)) {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, rayHit.point);

            LaserSound.transform.position = rayHit.point;


            IDamageble entity = null;

            rayHit.collider.TryGetComponent<Astronaut>(out Astronaut astr);
            rayHit.collider.TryGetComponent<EnemyAI>(out EnemyAI enem);
            if (astr != null)
                entity = astr;
            else if (enem != null)
                entity = enem;

            if (entity != null) {
               entity.Damage(100f);
               OnHitTarget?.Invoke();
            }
        }
        else {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + transform.forward * laserDistance);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray.origin, ray.direction * laserDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(rayHit.point, 0.23f);
    }
}