using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeControl : MonoBehaviour
{
    public ParticleSystem PsSmoke;
    public PlaneControl PlaneControl;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PsSmoke.isPlaying && !PlaneControl.IsBoosting)
        {
            PsSmoke.Stop();
        }

        if (!PsSmoke.isPlaying && PlaneControl.IsBoosting)
        {
            PsSmoke.Play();
        }
    }
}
