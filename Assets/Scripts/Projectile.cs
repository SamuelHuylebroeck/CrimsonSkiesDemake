using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public float BulletMaxVelocity;
    public float BulletLifeTime = 5f;

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
        var translocation = BulletMaxVelocity * Time.deltaTime * Vector3.forward;
        transform.Translate(translocation, Space.Self);
        BulletLifeTime -= Time.deltaTime;
        if (BulletLifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
