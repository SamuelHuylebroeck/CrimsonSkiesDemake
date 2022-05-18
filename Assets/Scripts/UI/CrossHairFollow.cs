using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossHairFollow : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera Cam;
    public PlaneGunneryRig GunneryRig;

    private RectTransform _crossHairTransfrom;

    void Start()
    {
        _crossHairTransfrom = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GunneryRig != null)
        {
            //Get the target location
            Vector3 targetPos = GunneryRig.transform.position + GunneryRig.transform.forward.normalized * GunneryRig.MaxConvergenceDistance;
            //Transform to screenspace
            Vector3 screenPos = Cam.WorldToScreenPoint(targetPos);
            //Update position
            _crossHairTransfrom.anchoredPosition.Set(screenPos.x, screenPos.y);
            //Sync roll
        }
    }
}
