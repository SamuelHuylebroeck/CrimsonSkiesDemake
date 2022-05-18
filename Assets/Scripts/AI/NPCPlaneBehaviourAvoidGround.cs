using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPlaneBehaviourAvoidGround : AbstractNPCPlaneBehaviour
{

    public float RollFactor = 1f;
    public float GroundDetectionAngle = 10f;
    public float GroundDetectionRange = 30f;

    public LayerMask GroundCollisionMask;


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
        var roll = context.planeControl.transform.localEulerAngles.z;
        if (roll > 180f) roll -= 360f;
        return new Vector3(-1f, 0, Mathf.Clamp(-roll * RollFactor, -1, 1));
    }

    public override string GetName()
    {
        return "Avoid Ground";
    }

    public override bool ShouldExecuteBehaviour(float dt, PlaneBehaviourContext context)
    {
        //Cast a ray forward and slightly downward to check for ground
        Transform localTf = context.planeControl.transform;
        Vector3 localDownRayDirection = Vector3.forward;
        localDownRayDirection = Quaternion.AngleAxis(GroundDetectionAngle, Vector3.right) * localDownRayDirection;


        Vector3 rayDirectionDown = localTf.TransformDirection(localDownRayDirection.normalized);
        Ray rayDown = new Ray(context.planeControl.transform.position, rayDirectionDown);



        bool projectedCollisionDown = Physics.Raycast(rayDown, GroundDetectionRange, GroundCollisionMask);
        Color colColor = projectedCollisionDown ? Color.green : Color.red;

        
        return projectedCollisionDown;
    }

}
