using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneInput : PlaneInput
{


    public GameObject target;
    public PlaneControl planeControl;

    public AbstractNPCPlaneBehaviour[] BehaviourStack;

    private PlaneControl _targetPlaneControl;

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

        int _selectedBehaviourIndex = 0;
        #region Construct context
        PlaneBehaviourContext context = new PlaneBehaviourContext(target.transform.position, planeControl, _targetPlaneControl);


        #endregion

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


        #region Use behaviour to calculate inputs Feed inputs

        AbstractNPCPlaneBehaviour behaviour = BehaviourStack[_selectedBehaviourIndex];

        print(behaviour.GetName());
        Vector3 steering = behaviour.CalculateSteering(Time.deltaTime, context);

        PitchInput = steering.x;
        YawInput = steering.y;
        RollInput = steering.z;

        BoostBrake = behaviour.CalculateBoostBreak(Time.deltaTime, context);
        PrimaryFire = behaviour.CalculatePrimaryFire(Time.deltaTime, context);
        #endregion
    }
}
