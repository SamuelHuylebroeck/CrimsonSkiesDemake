using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneStatus : MonoBehaviour
{
    #region boost
    [Tooltip("Maximum size of the boost gauge")]
    public float MaxBoostGauge = 1;
    [Tooltip("Boost gauge recovery, units/s")]
    public float BoostRecover = 0.2f;
    [Tooltip("Boost gauge drain when boosting, units/s")]
    public float BoostDrain = 0.33f;
    public bool CanBoost
    {
        get
        {
            return _currentBoostGauge > 0;
        }
    }

    public float Boost
    {
        get
        {
            return _currentBoostGauge;
        }

    }
    #endregion

    #region health and damage
    public float MaxHealth = 100;
    public float CurrentHealth = 100;

    public GameObject DamageThresholdExplosionTemplate;
    public GameObject DeathExplosion;

    private Stack<float> _thresholdStack;
    #endregion

    public PlaneInput input;

    public bool IsPlayer = false;

    public DropOnDeath DeathDrops;


    [SerializeField]
    private float _currentBoostGauge;

    void Start()
    {
        _currentBoostGauge = MaxBoostGauge;
        _thresholdStack = new Stack<float>();
        _thresholdStack.Push(0.2f);
        _thresholdStack.Push(0.5f);

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

    public void Damage(float damageAmount) 
    {
        CurrentHealth -= damageAmount;

        if (_thresholdStack.Count > 0)
        {
            float nextThreshold = _thresholdStack.Peek();
            if (CurrentHealth / MaxHealth < nextThreshold)
            {
                _thresholdStack.Pop();
                if(DamageThresholdExplosionTemplate != null)
                {
                    Instantiate(DamageThresholdExplosionTemplate, transform.position, transform.rotation);
                }

            }
        }


        if (CurrentHealth <= 0)
        {
            print(string.Format("Plane {0} should be dead", this.gameObject));
            if (DeathExplosion != null)
            {
                Instantiate(DeathExplosion, transform.position, transform.rotation);
            }
            if (DeathDrops != null)
            {
                DeathDrops.Drop();
            }

            GameObject.Destroy(gameObject);

        }

    }

    public void Repair(float repairFraction) 
    {
        CurrentHealth += Mathf.Clamp(CurrentHealth + repairFraction * MaxHealth, 0, MaxHealth);
    }
}
