using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDMarkerEnemy : MonoBehaviour
{
    public PlaneStatus connectedPlane;
    public PlaneControl playerPlane;
    public RectTransform rTf;
    public float MaxDistance = 800f;
    public Camera cam;

    public float maxScale = 0.2f;
    public float minScale = 0.01f;

    public Image image;
    public Sprite InCameraSprite;
    public Sprite OutOfCameraSprite;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool UpdateVisual()
    {
        if (connectedPlane == null)
        {
            return false;
        }
        
        float distance = Vector3.Distance(connectedPlane.transform.position, playerPlane.transform.position);
        Vector3 viewPortPos = cam.WorldToViewportPoint(connectedPlane.transform.position);
        float scale = Mathf.Lerp(minScale, maxScale, 1 - distance / MaxDistance);


        if (distance > MaxDistance) 
        {
            return false;
        }



        if (!TargetInForwardFrustrum(connectedPlane.transform.position, cam))
        {
            //Put the position on the canvas correct
            Vector3 ScreenPos = cam.ViewportToScreenPoint(new Vector3(Mathf.Clamp(viewPortPos.x, 0.05f, 0.95f), Mathf.Clamp(viewPortPos.y, 0.05f, 0.95f), 0f));
            if (TargetInBackFrustrum(connectedPlane.transform.position, cam))
            {
                Vector3 ScreenPosDirection = (ScreenPos - new Vector3(cam.pixelWidth / 2, cam.pixelHeight / 2,0)).normalized;
                ScreenPosDirection *= 2000;
                ScreenPos = cam.ViewportToScreenPoint(new Vector3(Mathf.Clamp(cam.pixelWidth / 2 + ScreenPosDirection.x, 0.05f, 0.95f), Mathf.Clamp(cam.pixelHeight/2 + ScreenPosDirection.y, 0.05f, 0.95f), 0f));

            }
            
            rTf.anchoredPosition = new Vector2(ScreenPos.x, ScreenPos.y);
            image.sprite = OutOfCameraSprite;
            rTf.localScale = new Vector3(scale,scale,scale);

            Vector2 AnchorPosRelativeToCenter = new Vector2(ScreenPos.x, ScreenPos.y) - new Vector2(cam.pixelWidth / 2, cam.pixelHeight / 2);
            rTf.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, AnchorPosRelativeToCenter) + 90);//Magic number correction for sprite orientation


        }
        else
        {
            //Put the position on the canvas correct
            Vector3 ScreenPos = cam.ViewportToScreenPoint(viewPortPos);
            rTf.anchoredPosition = new Vector2(ScreenPos.x, ScreenPos.y);
            image.sprite = InCameraSprite;
            rTf.localScale = new Vector3(scale, scale, scale);
            rTf.rotation = Quaternion.identity;
        }


        
        return true;
    }


    public bool TargetInForwardFrustrum(Vector3 TargetWorldPosition, Camera cam)
    {
        Vector3 viewPortPos = cam.WorldToViewportPoint(connectedPlane.transform.position);
        bool targetOutsideOfViewport = viewPortPos.x > 1 || viewPortPos.x < 0 || viewPortPos.y > 1 || viewPortPos.y < 0;
        if (!targetOutsideOfViewport)
        {
            //Check angle
            Vector3 targetDir = (TargetWorldPosition - cam.transform.position).normalized;
            float TargetAngle = Vector3.Angle(cam.transform.forward, targetDir);

            return TargetAngle < cam.fieldOfView;
        }
        else {
            return false;
        }
    }

    public bool TargetInBackFrustrum(Vector3 TargetWorldPosition, Camera cam)
    {
        Vector3 viewPortPos = cam.WorldToViewportPoint(connectedPlane.transform.position);
        bool targetOutsideOfViewport = viewPortPos.x > 1 || viewPortPos.x < 0 || viewPortPos.y > 1 || viewPortPos.y < 0;
        if (!targetOutsideOfViewport)
        {
            //Check angle
            Vector3 targetDir = (TargetWorldPosition - cam.transform.position).normalized;
            float TargetAngle = Vector3.Angle(-1*cam.transform.forward, targetDir);
            return TargetAngle < cam.fieldOfView && cam.transform.InverseTransformPoint(connectedPlane.transform.position).z < 0;
        }
        else
        {
            return false;
        }
    }

}
