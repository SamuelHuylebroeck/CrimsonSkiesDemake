using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileExplosion : MonoBehaviour
{
    // Start is called before the first frame update
    public float ExplosionRadius;
    public float ExplosionDamage;
    public GameObject ExplosionTemplate;

    public GameObject Missile;

    public LayerMask explosionMask;

    public float ArmDelay;

    public float MissileLifeTime = 60;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ArmDelay -= Time.deltaTime;
        MissileLifeTime -= Time.deltaTime;
        if (MissileLifeTime < 0)
        {
            Explode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ArmDelay <= 0)
        {

            //print("Trigger hit with " + other);
            Explode();

        }
    }

    private void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, ExplosionRadius, explosionMask);
        foreach (Collider col in hits)
        {
            //print("Explosion hit on " + col.gameObject);
            PlaneStatus ps = col.gameObject.GetComponentInChildren<PlaneStatus>();

            if (ps != null)
            {
                ps.Damage(ExplosionDamage);
            }
        }
        if (ExplosionTemplate != null)
        {
            Instantiate(ExplosionTemplate, transform.position, transform.rotation);
        }
        else
        {
            print("Missile has no explosion template present");
        }

        Destroy(Missile);
    }
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
    }
}
