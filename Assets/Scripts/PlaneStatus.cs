using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneStatus : MonoBehaviour
{
    // Start is called before the first frame update
    
    #region boost
    [Tooltip("Maximum size of the boost gauge")]
    public float MaxBoostGauge = 1;
    [Tooltip("Boost gauge recovery, units/s")]
    public float BoostRecover = 0.2f;
    [Tooltip("Boost gauge drain when boosting, units/s")]
    public float BoostDrain = 0.33f;

    public PlaneInput input;

    public bool CanBoost
    {
        get 
        {
            return _currentBoostGauge > 0;
        }
    }
    #endregion

    [SerializeField]
    private float _currentBoostGauge;

    void Start()
    {
        _currentBoostGauge = MaxBoostGauge;
    }

    // Update is called once per frame
    void Update()
    {
        if (input.BoostBrake > 0) {
            _currentBoostGauge -= BoostDrain * Time.deltaTime;
        } else {
            _currentBoostGauge += BoostRecover * Time.deltaTime;
        }

        _currentBoostGauge = Mathf.Clamp(_currentBoostGauge, 0f, MaxBoostGauge);
    }
}
