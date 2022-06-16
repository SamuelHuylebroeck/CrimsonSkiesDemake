using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneControl : MonoBehaviour
{
    public PlaneInput PInput;
    public CharacterController CController;
    public PlaneStatus PStatus;

    [Tooltip("Maximum forward speed, units per second")]
    public float MaxForwardSpeed = 25f;

    public float BoostMultiplier = 1.2f;

    public float BrakeMultiplier = 0.667f;

    public bool IsBoosting {
        get {
            return PStatus.CanBoost && PInput.BoostBrake > 0f;
        }
    }

    [Tooltip("Maximum change in pitch, degrees per second")]
    public float MaxPitchSpeed = 60f;
    [Tooltip("Maximum change in yaw, degrees per second")]
    public float MaxYawSpeed = 30f;
    [Tooltip("Maximum change in pitch, degrees per second")]
    public float MaxRollSpeed = 90f;

    public float PitchAcceleration = 120f;
    public float YawAcceleration = 60f;
    public float RollAcelleration = 120f;

    public float maxYawRoll = 90f;
    public Transform PlaneModel;

    public float ForwardAcceleration = 10f;
    public float ForwardDeacceleration = 20f;

    private float _currentPitchSpeed, _currentYawSpeed, _currentRollSpeed;
    private float _currentForwardSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        _currentPitchSpeed = 0;
        _currentRollSpeed = 0;
        _currentYawSpeed = 0;
        _currentForwardSpeed = MaxForwardSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //Move the plane forwards it's speed
        CController.Move(transform.forward.normalized * _currentForwardSpeed*Time.deltaTime);
        #region rotations
        //Apply pitch
        float pitch = PInput.PitchInput;
        if(!Mathf.Approximately(pitch, 0f))
        {

            _currentPitchSpeed = Mathf.Lerp(_currentPitchSpeed, pitch * MaxPitchSpeed, PitchAcceleration * Time.deltaTime);
            transform.Rotate(_currentPitchSpeed*Time.deltaTime, 0, 0, Space.Self);

        }
        

        //Apply yaw
        float yaw = PInput.YawInput;
        if (!Mathf.Approximately(yaw, 0f))
        {
            _currentYawSpeed = Mathf.Lerp(_currentYawSpeed, yaw * MaxYawSpeed, YawAcceleration * Time.deltaTime);
            transform.Rotate(0, _currentYawSpeed * Time.deltaTime, 0, Space.Self);

            //Apply yaw roll
            PlaneModel.transform.Rotate(new Vector3(0, 0, -1*yaw*MaxRollSpeed * Time.deltaTime), Space.Self);
            //Clamp to max values
            Vector3 lEV = PlaneModel.transform.localRotation.eulerAngles;
            lEV = new Vector3(lEV.x, lEV.y, Mathf.Clamp(lEV.z < 180 ? lEV.z : lEV.z-360, -1 * maxYawRoll, maxYawRoll));
            PlaneModel.transform.localEulerAngles = lEV;
        }
        else {
            //Gradually reset yaw roll
            float angle = Quaternion.Angle(PlaneModel.transform.localRotation, Quaternion.identity);
            float t = Time.deltaTime / (angle / (MaxRollSpeed));

            PlaneModel.transform.localRotation = Quaternion.Slerp(PlaneModel.transform.localRotation, Quaternion.identity, t);


        
        }
        
        //Apply roll
        float roll = PInput.RollInput;
        if (!Mathf.Approximately(roll, 0f)) {
            _currentRollSpeed = Mathf.Lerp(_currentRollSpeed, roll * MaxRollSpeed, RollAcelleration * Time.deltaTime);
            transform.Rotate(0,0, _currentRollSpeed * Time.deltaTime,Space.Self);
        }
        #endregion

        #region Speedupdates

        float targetSpeed = MaxForwardSpeed;

        if (!Mathf.Approximately(PInput.BoostBrake, 0.0f))
        {
            if (PInput.BoostBrake > 0f)
            {
                if (PStatus.CanBoost)
                {
                    targetSpeed *= BoostMultiplier;
                }
                else {
                   //print("Boost empty");
                }
            }
            else
            {
                targetSpeed *= BrakeMultiplier;
            }
        }
        //Accelerate
        if (_currentForwardSpeed < targetSpeed)
        {
            _currentForwardSpeed = Mathf.Min(_currentForwardSpeed + ForwardAcceleration * Time.deltaTime, targetSpeed);
        
        }
        //De-accelerate
        if (_currentForwardSpeed > targetSpeed)
        {
            _currentForwardSpeed = Mathf.Max(_currentForwardSpeed - ForwardDeacceleration * Time.deltaTime, targetSpeed);
        }
        #endregion


        //print(string.Format("Forward {2} - Yaw {0}, Pitch {1} - Rotation {3}", yaw, pitch, transform.forward.normalized, transform.rotation));

    }
}
