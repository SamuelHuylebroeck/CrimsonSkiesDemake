using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAttackRunPlaneInput : PlaneInput
{
    public GameObject targetPlane;

    //Based on https://vazgriz.com/503/creating-a-flight-simulator-in-unity3d-part-3/#more-503

    public float PitchUpThreshold = -15;
    public float FineSteeringAngle = 5;
    public float RollFactor = 0.01f;
    public float SteeringSpeed = 5;

    public PlaneControl planeControl;

    private Vector3 _lastInput;

    // Start is called before the first frame update
    void Start()
    {
        //Debug behaviour, when no target is present, lock on to PC
        if (targetPlane == null)
        {
            targetPlane = GameObject.FindGameObjectWithTag("Player");
            print("Player Acquired");
        }
        _lastInput = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {        
        Vector3 targetPosition = targetPlane.transform.position;

        Vector3 steering = CalculateSteering(Time.deltaTime, targetPosition);

        #region assign inputs
        PitchInput = steering.x;
        YawInput = steering.y;
        RollInput = steering.z;
        #endregion
    }


    Vector3 CalculateSteering(float dt, Vector3 targetPosition)
    {
        Vector3 steering = Vector3.zero;
        Vector3 targetPosLocal = transform.InverseTransformPoint(targetPosition);

        bool yawing = false, rolling = false;

        #region pitch
        Vector3 pitchError = new Vector3(0, targetPosLocal.y, targetPosLocal.z).normalized;
        float pitch = Vector3.SignedAngle(Vector3.forward, pitchError, Vector3.right);

        if (-pitch < PitchUpThreshold) pitch += 360f;
        steering.x = pitch;
        #endregion

        #region roll and yaw
        Vector3 rollError = new Vector3(targetPosLocal.x, targetPosLocal.y, 0).normalized;
        if (Vector3.Angle(Vector3.forward, targetPosLocal.normalized) < FineSteeringAngle)
        {
            steering.y = targetPosLocal.x;
            yawing = true;
        }
        else
        {
            float roll = Vector3.SignedAngle(Vector3.up, rollError, Vector3.forward);
            steering.z = roll * RollFactor;
            rolling = true;
        }

        #endregion

        #region Clamp and scale to plane capabilities
        steering.x = Mathf.Clamp(steering.x / planeControl.MaxPitchSpeed, -1, 1);
        steering.y = Mathf.Clamp(steering.y / planeControl.MaxYawSpeed, -1, 1);
        steering.z = Mathf.Clamp(steering.z / planeControl.MaxRollSpeed, -1, 1);
        #endregion
        
        #region Apply a delay to the new input
        steering = Vector3.MoveTowards(_lastInput, steering, SteeringSpeed * dt);
        _lastInput = steering;
        #endregion

        print(string.Format("Steering: {0}, Rolling: {1}, Yawing: {2}", steering, rolling, yawing));
        DrawDebugLines(targetPosition);
        return steering;
    }

    void DrawDebugLines(Vector3 targetPosition) {

        Debug.DrawLine(transform.position, targetPosition, Color.blue, 5f, true);
    }
}
