using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneGunneryRig : MonoBehaviour
{
    public GameObject BulletTemplatePrimary;
    public float ShotCooldownPrimary;
    public FiringPoint[] FiringPointsPrimary;

    public GameObject BulletTemplateSecondary;
    public float ShotCooldownSecondary;
    public FiringPoint[] FiringPointsSecondary;
    
    [Tooltip("Cost to fire a single shot of seconday weapon, expressed as a fraction")]
    public float SecondaryAmmoCost;

    public float MaxConvergenceDistance;
    public float MinConvergenceDistance;
    public LayerMask ConvergenceMask;

    public PlaneInput PlaneInput;

    private int _currentFiringPointIndexPrimary;
    private bool _canFirePrimary;
    private float _currentCooldownPrimary;

    private int _currentFiringPointIndexSecondary;
    private bool _canFireSecondary;
    private float _currentCooldownSecondary;

    private float _secondaryAmmo;

    public int CurrentSecondaryAmmo
    {
        get {
           return Mathf.FloorToInt(1 / SecondaryAmmoCost * _secondaryAmmo);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentFiringPointIndexPrimary = 0;
        _canFirePrimary = true;

        _currentFiringPointIndexSecondary = 0;
        _canFireSecondary = true;

        _secondaryAmmo = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_canFirePrimary)
        {
            _currentCooldownPrimary -= Time.deltaTime;
            if (_currentCooldownPrimary < 0)
            {
                _canFirePrimary = true;
            }
        }

        if (!_canFireSecondary && _secondaryAmmo > SecondaryAmmoCost)
        {
            _currentCooldownSecondary -= Time.deltaTime;
            if (_currentCooldownSecondary < 0)
            {
                _canFireSecondary = true;
            }
        }


        if (PlaneInput.PrimaryFire && _canFirePrimary)
        {
            FireBullet();
        }

        if (PlaneInput.SecondayFire && _canFireSecondary)
        {
            FireSecondary();
        }
    }

    void FireBullet() {
        _canFirePrimary = false;
        _currentCooldownPrimary += ShotCooldownPrimary;

        Transform currentFiringPointTf = FiringPointsPrimary[_currentFiringPointIndexPrimary].transform;
        Vector3 convergencePoint = calculateConvergencePoint(MinConvergenceDistance, MaxConvergenceDistance, ConvergenceMask);
        
        currentFiringPointTf.LookAt(convergencePoint);
        Vector3 firingPos = currentFiringPointTf.transform.position;

        GameObject bullet = Instantiate(BulletTemplatePrimary, firingPos, currentFiringPointTf.rotation);
        FiringPointsPrimary[_currentFiringPointIndexPrimary].Fire();

        _currentFiringPointIndexPrimary = (_currentFiringPointIndexPrimary + 1) % FiringPointsPrimary.Length;
    }

    void FireSecondary() {
        _canFireSecondary = false;
        _currentCooldownSecondary += ShotCooldownSecondary;
        _secondaryAmmo -= SecondaryAmmoCost;

        Transform currentFiringPointTf = FiringPointsSecondary[_currentFiringPointIndexSecondary].transform;
        Vector3 convergencePoint = calculateConvergencePoint(MinConvergenceDistance, MaxConvergenceDistance, ConvergenceMask);

        currentFiringPointTf.LookAt(convergencePoint);
        Vector3 firingPos = currentFiringPointTf.transform.position;

        GameObject bullet = Instantiate(BulletTemplateSecondary, firingPos, currentFiringPointTf.rotation);
        FiringPointsSecondary[_currentFiringPointIndexSecondary].Fire();

        _currentFiringPointIndexSecondary = (_currentFiringPointIndexSecondary + 1) % FiringPointsSecondary.Length;

    }

    Vector3 calculateConvergencePoint(float minDistance, float maxDistance, LayerMask rayMask)
    {
        Ray planeForward = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(planeForward, out hitInfo, maxDistance, rayMask.value);
        if (!hit)
        {
            return transform.position + transform.forward.normalized * maxDistance;
        }
        else {
            return transform.position + transform.forward.normalized * Mathf.Clamp(hitInfo.distance, minDistance, maxDistance);
        }
        

    }
    public void Reload(float ReloadFraction) 
    {
        _secondaryAmmo = Mathf.Clamp(_secondaryAmmo + ReloadFraction, 0, 1);
    }
}
