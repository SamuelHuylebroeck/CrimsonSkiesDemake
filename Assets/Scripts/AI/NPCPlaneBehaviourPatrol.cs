using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPlaneBehaviourPatrol : AbstractNPCPlaneBehaviour
{
    private Vector3 _lastInput;

    //Based on https://vazgriz.com/503/creating-a-flight-simulator-in-unity3d-part-3/#more-503

    #region Steering configuration
    public float PitchUpThreshold = -15;
    public float FineSteeringAngle = 5;
    public float RollFactor = 1f;
    public float SteeringSpeed = 5;
    #endregion

    #region patrol configuration

    public Transform[] PatrolPoints;
    public float GoalDistance = 10;
    private int _currentPatrolTargetIndex;
    public float AggroRange = 500;
    #endregion




    void Start()
    {
        _lastInput = Vector3.zero;
        _currentPatrolTargetIndex = 0;

    }
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

        #region Calculate intercept position
        Vector3 interceptPos = PatrolPoints[_currentPatrolTargetIndex].position;
        Debug.DrawLine(planeControl.transform.position, interceptPos, Color.blue);
        #endregion

        Vector3 targetPosLocal = transform.InverseTransformPoint(interceptPos);

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
            //When yawing, roll to be level
            if (pitch < 5f)
            {
                var roll = planeControl.transform.localEulerAngles.z;
                if (roll > 180f) roll -= 360f;
                steering.z = -roll * RollFactor;
            }

        }
        else
        {
            float roll = Vector3.SignedAngle(Vector3.up, rollError, Vector3.forward);
            steering.z = -roll * RollFactor;
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

        #region Check if the current patrol point is reached and if we should move to the next
        if (Vector3.Distance(planeControl.transform.position, interceptPos) < GoalDistance)
        {
            _currentPatrolTargetIndex = (_currentPatrolTargetIndex+1) % PatrolPoints.Length;
        }

        #endregion
        return steering;
    }

    public override string GetName()
    {
        return "Patrol - " + PatrolPoints[_currentPatrolTargetIndex];
    }

    public override bool ShouldExecuteBehaviour(float dt, PlaneBehaviourContext context)
    {
        if (PatrolPoints.Length < 1)
        {
            return false;
        }
        
        float distanceToTarget = Vector3.Distance(context.planeControl.transform.position, context.TargetPosition);
        return distanceToTarget > AggroRange;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, AggroRange);
    }
}
