using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                CameraZ = CameraZPositions[2];

            }
            else
            {
                CameraZ = CameraZPositions[0];
            }
        
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
        #region sync position
        transform.position = plane.transform.position;
        #endregion

        #region sync rotation
        GradualSyncRotation(plane.transform.rotation, SpeedRotation);
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
