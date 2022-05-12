using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractNPCPlaneBehaviour: MonoBehaviour
{

    public abstract bool ShouldExecuteBehaviour(float dt, PlaneBehaviourContext context);

    public abstract Vector3 CalculateSteering(float dt, PlaneBehaviourContext context);

    public abstract float CalculateBoostBreak(float dt, PlaneBehaviourContext context);

    public abstract bool CalculatePrimaryFire(float dt, PlaneBehaviourContext context);

    public abstract string GetName();


}


public class PlaneBehaviourContext
{
    public Vector3 TargetPosition;
    public PlaneControl planeControl;
    public PlaneControl targetPlaneControl;

    public Vector3 TargetVelocity
    {
        get {
            return targetPlaneControl.transform.forward * targetPlaneControl.MaxForwardSpeed;
        }
    }

    private CharacterController _targetCControl;

    public PlaneBehaviourContext(Vector3 targetPos, PlaneControl planeControl, PlaneControl targetPlaneControl = null)
    {
        this.TargetPosition = targetPos;
        this.planeControl = planeControl;
        this.targetPlaneControl = targetPlaneControl;
        if (targetPlaneControl != null)
        {
            _targetCControl = targetPlaneControl.CController;
        }
        else {
            _targetCControl = null;
        }
            

    }

}
