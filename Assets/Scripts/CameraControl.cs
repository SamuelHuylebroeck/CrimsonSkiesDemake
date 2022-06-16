using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CameraControl : MonoBehaviour
{
    
    // Start is called before the first frame update
    public Transform CameraY;
    public Transform Camera;
    public float SpeedRotation;
    public PlaneControl plane;
    public PlayerPlaneInput input;

    public float CameraZAdjustSpeed;

    public float[] CameraZPositions =  new float[] { -7,-10, -15};

    private float _targetCameraZDistance;

    private float _cameraAdditonalRotation;

    private PlaneStatus _focusTarget;
    public float FocusRange = 400;
    public LayerMask FocusMask;

    public Image CrossHairs;

    public float CameraZ {
        get
        {
            return Camera.localPosition.z;
        }

        set 
        {

            _targetCameraZDistance = value;
        }
    }



    void Start()
    {
        _targetCameraZDistance = CameraZPositions[1];
        _cameraAdditonalRotation = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Approximately(input.BoostBrake, 0.0f))
        {
            CameraZ = CameraZPositions[1];
        }
        else {
            if (input.BoostBrake > 0f)
            {
                if (plane.IsBoosting) {

                    CameraZ = CameraZPositions[2];
                } else
                {
                    CameraZ = CameraZPositions[1];
                }

            }
            else
            {
                CameraZ = CameraZPositions[0];
            }

        }


        _cameraAdditonalRotation = 0;
        if (Input.GetAxis("CameraLookBack") < 0)
        {
            _cameraAdditonalRotation = 180;
        }else{ 
            float additionalRotInput = Input.GetAxis("CameraSwivel");
            if (additionalRotInput != 0.0)
            {
                _cameraAdditonalRotation = additionalRotInput > 0.0 ? 90 : -90;
            }
        }

        if (Input.GetButtonDown("CameraTrack"))
        {
            PulseForTarget();
        }


    }

    public bool GradualSyncRotation(Quaternion target, float rotationSpeed) 
    {
        #region rotation
        float angle = Quaternion.Angle(transform.rotation, target);
        float t = Time.deltaTime / (angle / rotationSpeed);

        transform.rotation = Quaternion.Slerp(transform.rotation, target, t);
        #endregion

        return t >= 1;

    }

    private void LateUpdate()
    {
        if (plane != null)
        {
            #region sync position
            transform.position = plane.transform.position;
            #endregion


            CrossHairs.enabled = true;

            if (_cameraAdditonalRotation != 0.0 || Input.GetButton("CameraTrack"))
            {
                CrossHairs.enabled = false;
            }
            #region sync rotation
            Quaternion targetRotation = plane.transform.rotation;

            if (Input.GetButton("CameraTrack") && _focusTarget != null)
            {
                var oldRotation = transform.rotation;
                transform.LookAt(_focusTarget.transform);
                targetRotation = transform.rotation;
                transform.rotation = oldRotation;
            }
            else {
                targetRotation *= Quaternion.AngleAxis(_cameraAdditonalRotation, Vector3.up);
            }


            GradualSyncRotation(targetRotation, SpeedRotation);
            #endregion

            #region update camera Z-pos

            Vector3 targetPos = new Vector3(Camera.localPosition.x, Camera.localPosition.y, _targetCameraZDistance);
            float distance = Vector3.Distance(Camera.localPosition, targetPos);
            float timeToReach = distance / CameraZAdjustSpeed;
            float t = Time.deltaTime / timeToReach;
            Camera.transform.localPosition = Vector3.Lerp(Camera.localPosition, targetPos, t);
            #endregion
        }
    }

    private void PulseForTarget() 
    {
        Collider[] targetsInRange = Physics.OverlapSphere(plane.transform.position, FocusRange, FocusMask);
        float targetRange = FocusRange + 1;
        PlaneStatus closest = null;
        foreach (Collider col in targetsInRange)
        {
            PlaneStatus ps = col.gameObject.GetComponent<PlaneStatus>();
            if (ps != null && !ps.IsPlayer)
            {
                float distance = Vector3.Distance(plane.transform.position, ps.transform.position);
                if (distance < targetRange) {
                    targetRange = distance;
                    closest = ps;
                }
            }
        
        }
        print(closest);
        _focusTarget = closest;
    }
}
