using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringPoint : MonoBehaviour
{

    public GameObject FireEffectTemplate;

    public void Update()
    {
    }

    public void Fire() {
        Instantiate(FireEffectTemplate, transform.position, transform.rotation);
    }




}
