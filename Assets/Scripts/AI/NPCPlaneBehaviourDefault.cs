using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPlaneBehaviourDefault : AbstractNPCPlaneBehaviour
{
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
        return Vector3.zero;
    }

    public override bool ShouldExecuteBehaviour(float dt, PlaneBehaviourContext context)
    {
        return true;
    }

    public override string GetName()
    {
        return "Default - No Steering";
    }
}
