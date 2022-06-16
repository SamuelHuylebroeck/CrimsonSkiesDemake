using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPlaneInput : PlaneInput
{


    public GameObject target;
    public PlaneControl planeControl;

    public AbstractNPCPlaneBehaviour[] BehaviourStack;

    protected PlaneControl _targetPlaneControl;

    // Start is called before the first frame update
    void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
            _targetPlaneControl = target.GetComponent<PlaneControl>();
            print("Player Acquired");
        }
    }

    // Update is called once per frame
    void Update()
    {
        #region Construct context
        PlaneBehaviourContext context = CreateContext();
        #endregion
        #region Use behaviour to calculate inputs Feed inputs
        int index = SelectBehaviour(context);
        FeedInputs(context, index);
        #endregion
    }

    protected PlaneBehaviourContext CreateContext() {
        PlaneBehaviourContext context = new PlaneBehaviourContext(target != null? target.transform.position : Vector3.zero, planeControl, target != null ? _targetPlaneControl : null);
        return context;
    }

    protected int SelectBehaviour(PlaneBehaviourContext context)
    {
        int _selectedBehaviourIndex = 0;
        #region Select behaviour
        //Always make sure a NPCPlaneBehaviourDefault is present at the end of the stack
        while (_selectedBehaviourIndex < BehaviourStack.Length)
        {
            if (BehaviourStack[_selectedBehaviourIndex].ShouldExecuteBehaviour(Time.deltaTime, context))
            {
                break;
            }

            _selectedBehaviourIndex++;

        }
        #endregion
        return _selectedBehaviourIndex;
    }

    protected void FeedInputs(PlaneBehaviourContext context, int BehaviourIndex)
    {
        AbstractNPCPlaneBehaviour behaviour = BehaviourStack[BehaviourIndex];

        print(this.gameObject + ": " + behaviour.GetName());
        Vector3 steering = behaviour.CalculateSteering(Time.deltaTime, context);

        PitchInput = steering.x;
        YawInput = steering.y;
        RollInput = steering.z;

        BoostBrake = behaviour.CalculateBoostBreak(Time.deltaTime, context);
        PrimaryFire = behaviour.CalculatePrimaryFire(Time.deltaTime, context);
    }
}

