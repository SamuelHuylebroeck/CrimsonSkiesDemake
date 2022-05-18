using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneInput : NPCPlaneInput
{
    // Start is called before the first frame update
    void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
            _targetPlaneControl = target.GetComponent<PlaneControl>();
            print("Player Acquired");
        }
    }
}
