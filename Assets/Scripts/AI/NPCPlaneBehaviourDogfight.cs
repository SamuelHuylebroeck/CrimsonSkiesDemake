using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPlaneBehaviourDogfight : AbstractNPCPlaneBehaviour
{
    private Vector3 _lastInput;

    //Based on https://vazgriz.com/503/creating-a-flight-simulator-in-unity3d-part-3/#more-503

    #region Steering configuration
    public float PitchUpThreshold = -15;
    public float FineSteeringAngle = 5;
    public float RollFactor = 1f;
    public float SteeringSpeed = 5;
    #endregion

    #region Fire control configuration
    public float MaxPrimaryRange = 200;
    public float MinFiringRange = 20;
    public float FireAngle = 1;
    public float BulletSpeed = 50;
    #endregion

    #region boost and brake
    public bool CanBoostBrake = false;
    public float BoostDistance;
    public float BrakeDistance;
    #endregion

    public bool Intercept = false;

    #region Dogfighting Endurance setup
    public bool UseEndurance = true;
    public float MaxEndurance = 10;
    [SerializeField]
    private float _currentEndurance;
    [SerializeField]
    private bool _onEnduranceCooldown;

    public float DogfightEnduranceDistance;
    #endregion



    void Start()
    {
        _lastInput = Vector3.zero;
        _currentEndurance = MaxEndurance;
        _onEnduranceCooldown = false;
    }
    public override float CalculateBoostBreak(float dt, PlaneBehaviourContext context)
    {
        if (!CanBoostBrake) return 0.0f;
        if (Vector3.Distance(transform.position, context.TargetPosition) > BoostDistance) return 1.0f;
        if (Vector3.Distance(transform.position, context.TargetPosition) < BrakeDistance) return -1.0f;
        return 0.0f;
    }

    public override bool CalculatePrimaryFire(float dt, PlaneBehaviourContext context)
    {
        bool fire = false;
        Transform planeTf = context.planeControl.transform;
        float distanceToTarget = Vector3.Distance(context.planeControl.transform.position, context.TargetPosition);
        if (distanceToTarget >= MinFiringRange && distanceToTarget <= MaxPrimaryRange)
        {

            #region Calculate intercept position
            Vector3 interceptPos = context.TargetPosition;
            if (Intercept && context.TargetVelocity.magnitude != 0.0f)
            {
                interceptPos = MathUtilities.FirstOrderIntercept(context.planeControl.transform.position,context.planeControl.transform.forward * context.planeControl.MaxForwardSpeed, BulletSpeed, context.TargetPosition, context.TargetVelocity);
            }
            #endregion

            Vector3 targetLocalSpace = planeTf.InverseTransformPoint(interceptPos);
            float angle = Vector3.Angle(Vector3.forward, targetLocalSpace.normalized);
            if (angle < FireAngle)
            {
                fire = true;
            }
        }
        return fire;
    }

    public override Vector3 CalculateSteering(float dt, PlaneBehaviourContext context)
    {
        PlaneControl planeControl = context.planeControl;
        Vector3 steering = Vector3.zero;

        #region Calculate intercept position
        Vector3 interceptPos = context.TargetPosition;
        if(Intercept && context.TargetVelocity.magnitude != 0.0f)
        {
            interceptPos = MathUtilities.FirstOrderIntercept(planeControl.transform.position, planeControl.transform.forward * planeControl.MaxForwardSpeed, BulletSpeed, context.TargetPosition, context.TargetVelocity);

        }
        Debug.DrawLine(planeControl.transform.position, interceptPos, Color.blue);
        Debug.DrawLine(context.TargetPosition, interceptPos, Color.green);
        #endregion

        Vector3 targetPosLocal = transform.InverseTransformPoint(interceptPos);

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
            //When yawing, roll to be level
            if(pitch < 5f)
            {

                float roll = planeControl.transform.localEulerAngles.z;
                if (roll > 180f) roll -= 360f;
                steering.z = -roll*RollFactor;
            }

        }
        else
        {
            float roll = Vector3.SignedAngle(Vector3.up, rollError, Vector3.forward);
            steering.z = -roll * RollFactor;
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

        #region Endurance Update
        if(Vector3.Distance(transform.position, context.TargetPosition) < DogfightEnduranceDistance)
        {
            _currentEndurance -= Time.deltaTime * 2;
        }

        #endregion

        //print(string.Format("Steering: {0}, Rolling: {1}, Yawing: {2}", steering, rolling, yawing));
        return steering;
    }

    public override string GetName()
    {
        return "Dogfight";
    }

    public override bool ShouldExecuteBehaviour(float dt, PlaneBehaviourContext context)
    {
        bool EnduranceCheck = !UseEndurance || (_currentEndurance > 0 && !_onEnduranceCooldown);

        if (!_onEnduranceCooldown && _currentEndurance <= 0)
        {
            _onEnduranceCooldown = true;
        }
        _currentEndurance = Mathf.Min(_currentEndurance + Time.deltaTime, MaxEndurance);

        if (_onEnduranceCooldown && _currentEndurance >= MaxEndurance)
        {
            _onEnduranceCooldown = false;
        }

        //Dirty but quick fix on the position check
        return context.TargetPosition != Vector3.zero && EnduranceCheck;
    }
}
