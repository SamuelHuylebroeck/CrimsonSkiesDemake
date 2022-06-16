using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMissilePlaneInput : NPCPlaneInput
{

    public float SensorInterval;
    public float SensorRange;
    public LayerMask SensorMask;

    private float _currentSensorCount;
  
    
    // Start is called before the first frame update
    void Start()
    {
        //if (target == null)
        //{
        //    target = GameObject.FindGameObjectWithTag("Enemy");
        //    _targetPlaneControl = target.GetComponent<PlaneControl>();
        //    print("Enemy Acquired");

        //}
        _currentSensorCount = 0;
    }

    void Update()
    {
        #region Scan for targets
        _currentSensorCount -= Time.deltaTime;
        if (_currentSensorCount < 0)
        {
            SweepSensorForTarget();
            _currentSensorCount += SensorInterval;
        }
        #endregion

        #region Construct context
        PlaneBehaviourContext context = CreateContext();
        #endregion
        #region Use behaviour to calculate inputs Feed inputs
        int index = SelectBehaviour(context);
        FeedInputs(context, index);
        #endregion
    }

    void SweepSensorForTarget() { 
        Collider[] candidates = Physics.OverlapSphere(transform.position, SensorRange, SensorMask.value);
        foreach (Collider candidate in candidates)
        {
            //Ideally, we'd sort these based on distance so we home in on the closest, but clustered targets are going to be rare so skipping that atm
            PlaneStatus candidateStatus;
            if (candidate.TryGetComponent<PlaneStatus>(out candidateStatus))
            {
                if (!candidateStatus.IsPlayer)
                {
                    //Check angle
                    //Vector3 TargetLocal = transform.InverseTransformDirection(candidate.transform.position);
                    //float angle = Vector3.SignedAngle(Vector3.forward, TargetLocal, Vector3.right);
                    //if(Mathf.Abs(angle) < 160) //We want a bit more than a hemisphere for responsive missiles
                    //{
                    //    target = candidate.gameObject;
                    //    return;
                    //}
                    target = candidate.gameObject;
                    return;

                }
            }
        }
        target = null;

    }

    private void OnDrawGizmos()
    {
        Color targetAcquiredColour = target != null ? Color.green : Color.red;
        Gizmos.color = targetAcquiredColour;
        Gizmos.DrawWireSphere(transform.position, SensorRange);
    }
}
