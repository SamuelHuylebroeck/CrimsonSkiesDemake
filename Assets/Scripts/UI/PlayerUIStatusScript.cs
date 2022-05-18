using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIStatusScript : MonoBehaviour
{
    public PlaneStatus status;
    public PlaneGunneryRig gunneryRig;


    public Image HealthBar;
    public Image FuelBar;
    public Text AmmoText;
    
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        HealthBar.fillAmount = Mathf.Clamp(status.CurrentHealth / status.MaxHealth, 0, 1);
        FuelBar.fillAmount = Mathf.Clamp(status.Boost / status.MaxBoostGauge, 0, 1);
        AmmoText.text = "" + gunneryRig.CurrentSecondaryAmmo;
    }
}
