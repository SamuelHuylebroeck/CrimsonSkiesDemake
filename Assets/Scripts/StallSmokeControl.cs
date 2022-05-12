using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StallSmokeControl : MonoBehaviour
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
        if (PsSmoke.isPlaying && PlaneControl.PStatus.CanBoost)
        {
            PsSmoke.Stop();
        }

        if (!PsSmoke.isPlaying && PlaneControl.PInput.BoostBrake > 0.0f && !PlaneControl.PStatus.CanBoost)
        {
            PsSmoke.Play();
        }
    }
}
