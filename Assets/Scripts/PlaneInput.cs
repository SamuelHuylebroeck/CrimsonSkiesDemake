using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneInput : MonoBehaviour
{

    [Tooltip("Input for left-right movement")]
    public float YawInput;

    [Tooltip("Input for up-down movement")]
    public float PitchInput;

    [Tooltip("Input for rotational roll movement")]
    public float RollInput;

    public bool PrimaryFire;

    public float BoostBrake;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
