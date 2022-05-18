using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPlaneBehaviourLevelClimb : AbstractNPCPlaneBehaviour
{
    private Vector3 _lastInput;
    public float SteeringSpeed = 5f;
    public float ClimbAngle = 15f;
    public float StartPitchRollAngle = 5f;
    public float PitchUpThreshold = 15f;

    public override float CalculateBoostBreak(float dt, PlaneBehaviourContext context)
    {
        return 0.0f;
    }

    public override bool CalculatePrimaryFire(float dt, PlaneBehaviourContext context)
    {
        return false;
    }

    public override Vector3 CalculateSteering(float dt, PlaneBehaviourContext context)
    {
        PlaneControl planeControl = context.planeControl;
        Vector3 steering = Vector3.zero;
        var roll = planeControl.transform.localEulerAngles.z;
        if (roll > 180f) roll -= 360f;
        steering.z = -roll;

        if (Mathf.Abs(roll) <= StartPitchRollAngle)
        {
            //Get a target
            Vector3 target = new Vector3(planeControl.transform.forward.x, 0, planeControl.transform.forward.z).normalized;
            target = Quaternion.AngleAxis(ClimbAngle, Vector3.left)*target;

            Debug.DrawRay(context.planeControl.transform.position, target * 100, Color.green);
            Debug.DrawRay(context.planeControl.transform.position, planeControl.transform.forward*100, Color.yellow);

            Vector3 targetPosLocal = planeControl.transform.InverseTransformPoint(context.planeControl.transform.position + target*100);

            #region pitch
            Vector3 pitchError = new Vector3(0, targetPosLocal.y, targetPosLocal.z).normalized;
            float pitch = Vector3.SignedAngle(Vector3.forward, pitchError, Vector3.right);
            steering.x = pitch;
            #endregion
        }

        #region Clamp and scale to plane capabilities
        steering.x = Mathf.Clamp(steering.x / context.planeControl.MaxPitchSpeed, -1, 1);
        steering.y = Mathf.Clamp(steering.y / context.planeControl.MaxYawSpeed, -1, 1);
        steering.z = Mathf.Clamp(steering.z / context.planeControl.MaxRollSpeed, -1, 1);
        #endregion

        #region Apply a delay to the new input
        steering = Vector3.MoveTowards(_lastInput, steering, SteeringSpeed * dt);
        _lastInput = steering;
        #endregion

        return steering;
    }

    public override bool ShouldExecuteBehaviour(float dt, PlaneBehaviourContext context)
    {
        return true;
    }

    public override string GetName()
    {
        return "Level Climb - " + ClimbAngle;
    }

    // Start is called before the first frame update
    void Start()
    {
        _lastInput = Vector3.zero;
    }
}

