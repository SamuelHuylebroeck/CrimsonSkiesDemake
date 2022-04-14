using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneControl : MonoBehaviour
{
    public PlayerInput PInput;
    public CharacterController CController;

    [Tooltip("Maximum forward speed, units per second")]
    public float MaxForwardSpeed = 25f;

    [Tooltip("Maximum change in pitch, degrees per second")]
    public float MaxPitchSpeed = 60f;
    [Tooltip("Maximum change in yaw, degrees per second")]
    public float MaxYawSpeed = 30f;
    [Tooltip("Maximum change in pitch, degrees per second")]
    public float MaxRollSpeed = 60f;

    public float PitchAcceleration = 120f;
    public float YawAcceleration = 60f;
    public float RollAcelleration = 120f;

    private float _currentPitchSpeed, _currentYawSpeed, _currentRollSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        _currentPitchSpeed = 0;
        _currentRollSpeed = 0;
        _currentYawSpeed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Move the plane forwards it's speed
        CController.Move(transform.forward.normalized * MaxForwardSpeed*Time.deltaTime);
        //Apply pitch
        float pitch = PInput.PitchInput;
        if(!Mathf.Approximately(pitch, 0f))
        {

            _currentPitchSpeed = Mathf.Lerp(_currentPitchSpeed, pitch * MaxPitchSpeed, PitchAcceleration * Time.deltaTime);
            transform.Rotate(_currentPitchSpeed*Time.deltaTime, 0, 0, Space.Self);

        }
        

        //Apply yaw
        float yaw = PInput.YawInput;
        if (!Mathf.Approximately(yaw, 0f)) {
            _currentYawSpeed = Mathf.Lerp(_currentYawSpeed, yaw * MaxYawSpeed, YawAcceleration * Time.deltaTime);
            transform.Rotate(0, _currentYawSpeed * Time.deltaTime, 0, Space.Self);
        }
        
        //Apply rolld
        float roll = PInput.RollInput;
        if (!Mathf.Approximately(roll, 0f)) {
            _currentRollSpeed = Mathf.Lerp(_currentRollSpeed, roll * MaxRollSpeed, RollAcelleration * Time.deltaTime);
            transform.Rotate(0,0, _currentRollSpeed * Time.deltaTime,Space.Self);
        }


        //print(string.Format("Forward {2} - Yaw {0}, Pitch {1} - Rotation {3}", yaw, pitch, transform.forward.normalized, transform.rotation));

    }
}
