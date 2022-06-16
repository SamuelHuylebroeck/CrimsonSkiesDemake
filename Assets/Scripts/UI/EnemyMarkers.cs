using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMarkers : MonoBehaviour
{
    public Canvas PlayerHUD;
    public PlaneControl plane;
    public Camera HUDCam;
    public GameObject HUDMarkerTemplate;
    [SerializeField]
    private List<HUDMarkerEnemy> _markerList;

    public float ScanInterval = 1f;
    public float ScanRange = 800f;
    public LayerMask ScanMask;
    private float _timeUntilNextScan;
    
    // Start is called before the first frame update
    void Start()
    {
        _markerList = new List<HUDMarkerEnemy>();
        _timeUntilNextScan = ScanInterval;
    }

    // Update is called once per frame
    void Update()
    {
        _timeUntilNextScan -= Time.deltaTime;
        if (_timeUntilNextScan <= 0)
        {
            Scan();
            _timeUntilNextScan += ScanInterval;
        }
    }

    private void LateUpdate()
    {
        List<HUDMarkerEnemy> toRemove = new List<HUDMarkerEnemy>();
        foreach (HUDMarkerEnemy marker in _markerList)
        {
            bool present = marker.UpdateVisual();
            if (!present)
            {
                toRemove.Add(marker);
            }
        }
        foreach (HUDMarkerEnemy marker in toRemove)
        {
            _markerList.Remove(marker);
            Destroy(marker.gameObject);
        }
    }

    private void Scan()
    {

        Collider[] targetsInRange = Physics.OverlapSphere(plane.transform.position, ScanRange, ScanMask);
        //print("Scan - " +targetsInRange.Length);
        foreach (Collider col in targetsInRange)
        {
            PlaneStatus ps = col.gameObject.GetComponentInChildren<PlaneStatus>();
            if (ps != null && !ps.IsPlayer && !AlreadyHasMarker(ps))
            {
                //Create hud marker
                GameObject newMarkerGO = Instantiate(HUDMarkerTemplate, PlayerHUD.transform);
                HUDMarkerEnemy newMarker = newMarkerGO.GetComponentInChildren<HUDMarkerEnemy>();
                newMarker.connectedPlane = ps;
                newMarker.playerPlane = plane;
                newMarker.cam = HUDCam;
                _markerList.Add(newMarker);
                
            }

        }
    }

    private bool AlreadyHasMarker(PlaneStatus ps)
    {
        foreach (HUDMarkerEnemy marker in _markerList)
        {
            if (marker.connectedPlane == ps) {
                return true;
            }
        }
        return false;
    }
}
