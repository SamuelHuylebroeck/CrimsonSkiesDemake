using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateScript : MonoBehaviour
{
    public float RepairFraction = 0.33f;
    public float ReloadFraction = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        bool destroy = false;
        PlaneStatus ps = other.GetComponentInChildren<PlaneStatus>();
        if (ps != null && RepairFraction > 0)
        {
            ps.Repair(RepairFraction);
            destroy = true;
        }
        PlaneGunneryRig rig = other.GetComponentInChildren<PlaneGunneryRig>();
        if (rig != null && ReloadFraction > 0)
        {
            rig.Reload(ReloadFraction);
            destroy = true;
        }

        if (destroy) Destroy(gameObject);
    }
}
