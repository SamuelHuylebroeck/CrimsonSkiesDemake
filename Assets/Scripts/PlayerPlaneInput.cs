using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlaneInput : PlaneInput
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        YawInput = Input.GetAxis("Yaw");
        PitchInput = Input.GetAxis("Vertical");
        RollInput = -1 * Input.GetAxis("Horizontal");
        PrimaryFire = Input.GetAxis("Fire1") > 0;
        SecondayFire = Input.GetAxis("Fire2") > 0;
        BoostBrake = Input.GetAxis("BoostBrake");
    }
}
