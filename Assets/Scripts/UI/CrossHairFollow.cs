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

    private Vector3 _lastInput;
    private float smoothSpeed = 5f;

    void Start()
    {
        _crossHairTransfrom = GetComponent<RectTransform>();
        Vector3 targetPos = GunneryRig.transform.position + GunneryRig.transform.forward.normalized * GunneryRig.MaxConvergenceDistance;
        Vector3 screenPos = Cam.WorldToScreenPoint(targetPos);
        _lastInput = screenPos;
    }

    // Update is called once per frame
    void Update()
    {
        //Get the target location
        Vector3 targetPos = GunneryRig.transform.position + GunneryRig.transform.forward.normalized * GunneryRig.MaxConvergenceDistance;
        //Transform to screenspace
        Vector3 screenPos = Cam.WorldToScreenPoint(targetPos);
        //screenPos = Vector3.MoveTowards(_lastInput, screenPos, smoothSpeed * Time.deltaTime);
        _lastInput = screenPos;
        //Vector3 screenPos = Cam.ViewportToScreenPoint(new Vector3(0.5f, 0.45f, 0));
        //Update position
        _crossHairTransfrom.anchoredPosition = new Vector2(screenPos.x, screenPos.y);
    }
}
