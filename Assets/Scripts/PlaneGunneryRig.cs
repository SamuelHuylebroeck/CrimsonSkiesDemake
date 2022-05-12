using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneGunneryRig : MonoBehaviour
{
    public GameObject BulletTemplate;
    public float ShotCooldown;
    public FiringPoint[] FiringPoints;
    public float ConvergenceDistance;

    public PlaneInput PlaneInput;

    private int _currentFiringPointIndex;
    private bool _canFire;
    private float _currentCooldown;
    
    // Start is called before the first frame update
    void Start()
    {
        _currentFiringPointIndex = 0;
        _canFire = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_canFire)
        {
            _currentCooldown -= Time.deltaTime;
            if (_currentCooldown < 0)
            {
                _canFire = true;
            }
        }
        if (PlaneInput.PrimaryFire && _canFire)
        {
            FireBullet();
        }
    }
    void FireBullet() {
        //Get the current firing point
        _canFire = false;
        _currentCooldown = ShotCooldown;

        Transform currentFiringPointTf = FiringPoints[_currentFiringPointIndex].transform;
        Vector3 convergencePoint = new Vector3(0, 0, ConvergenceDistance);
        convergencePoint = currentFiringPointTf.TransformPoint(convergencePoint);
        
        currentFiringPointTf.LookAt(convergencePoint);
        Vector3 firingPos = currentFiringPointTf.transform.position;

        //var debugString = "World position: {0} - Rotation {1} - Index {2}";
        //print(string.Format(debugString, firingPos, currentFiringPoint.rotation, _currentFiringPointIndex));

        GameObject bullet = Instantiate(BulletTemplate, firingPos, currentFiringPointTf.rotation);
        FiringPoints[_currentFiringPointIndex].Fire();

        _currentFiringPointIndex = (_currentFiringPointIndex + 1) % FiringPoints.Length;
    }
}
