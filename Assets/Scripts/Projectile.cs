using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public float ProjectileMaxVelocity;
    public float ProjectileLifeTime = 5f;
    public GameObject BulletImpactTemplate;

    public float ProjectileDamage = 10;

    private Rigidbody _rb;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        var translocation = ProjectileMaxVelocity * Time.deltaTime * Vector3.forward;
        transform.Translate(translocation, Space.Self);
        ProjectileLifeTime -= Time.deltaTime;
        if (ProjectileLifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    { 
        if(BulletImpactTemplate != null)
        {
            Instantiate(BulletImpactTemplate, transform.position, transform.rotation);
        }
        float dam = ProjectileDamage;
        if (dam != 0.0f)
        {
            //Get the planestatus
            PlaneStatus ps = other.gameObject.GetComponentInChildren<PlaneStatus>();
            if (ps != null)
            {
                ps.Damage(dam);
            }
        }
        Destroy(gameObject);


    }
}
