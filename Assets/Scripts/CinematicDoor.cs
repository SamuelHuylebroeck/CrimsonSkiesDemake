using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicDoor : MonoBehaviour
{
    public GameObject SpawnTemplate;
    public Transform[] PatrolPoints;
    public float CheckInterval = 1;

    public GameObject SpawnPoint;

    private float _currentCheckInterval;

    public Rigidbody DoorBody;

    [SerializeField]
    private List<GameObject> _linkedPlanes;

    private bool _openedOnce;

    public bool IsOpen = false;
    public float DoorSpeed = 50f;
    public float OpenTime = 10f;
    private float _openTimeRemaining;
    private float _relativePosition;
    private bool moving;
    public Transform OpenLocation;
    private  Vector3 _closedLocation;

    public Rigidbody WallRb;

    public Camera DoorCam;
    public float CinematicTime = 5f;
    private float _remainingCinematicTime;
    public Canvas PlayerHUD;


    void Start()
    {
        _openedOnce = false;
        _currentCheckInterval = 1;
        _openTimeRemaining = OpenTime;
        _relativePosition = 0;
        moving = false;
        _closedLocation = transform.position;
        DoorCam.enabled = false;
        _remainingCinematicTime = CinematicTime;
    }

    // Update is called once per frame
    void Update()
    {
        _currentCheckInterval -= Time.deltaTime;
        
        if (_currentCheckInterval < 0)
        {
            _currentCheckInterval += CheckInterval;
            bool shouldOpen = CheckForOpen();
            if (shouldOpen)
            {
                Open();
            }
        }

        _openTimeRemaining -= Time.deltaTime;

        if (_openTimeRemaining < 0 && IsOpen)
        {
            Close();
        }

        if (_openedOnce) {
            _remainingCinematicTime -= Time.deltaTime;
            if (_remainingCinematicTime <= 0)
            {
                DoorCam.enabled = false;
                PlayerHUD.enabled = true;
            }

        }

    }

    private void LateUpdate()
    {
        #region position update
        if (moving)
        {
            float speed = IsOpen ?  -1*DoorSpeed : DoorSpeed;
            _relativePosition = Mathf.Clamp(_relativePosition + speed * Time.deltaTime, 0, 1);

            Vector3 position = Vector3.Lerp(_closedLocation, OpenLocation.position, _relativePosition);
            WallRb.MovePosition(position);
            //print(speed +"/" + _relativePosition + ":" + position);

            //Check for end
            if (!IsOpen && _relativePosition >= 1) 
            {
                //Door has fully opened
                //print("Door fully open");
                moving = false;
                IsOpen = true;
                _openTimeRemaining = OpenTime;
            }
            if (IsOpen && _relativePosition <= 0)
            {
                //Door has fully closed
                //print("Door fully closed");
                moving = false;
                IsOpen = true;
            }
        }
        #endregion
    }


    bool CheckForOpen()
    {
        bool result = true;
        foreach (GameObject plane in _linkedPlanes)
        {
            if (plane != null)
            {
                result = false;
            }
            else { 
            
            }
        
        }
        //print("Open - " + result);
        return result;
    }
    void Open()
    {
        if (!_openedOnce)
        {
            //Play Cutscene here
            PlayOpenCutscene();
        }

        //Start movement + spawn enemy
        GameObject NewPlane = Instantiate(SpawnTemplate, SpawnPoint.transform.position, SpawnPoint.transform.rotation);
        NPCPlaneBehaviourPatrol patrol;
        if (NewPlane.TryGetComponent<NPCPlaneBehaviourPatrol>(out patrol))
        {
            patrol.PatrolPoints = this.PatrolPoints;
        }
        _linkedPlanes.Add(NewPlane);
        moving = true;

    }

    void Close()
    {
        moving = true;
    }

    void PlayOpenCutscene()
    {
        DoorCam.enabled = true;
        _openedOnce = true;
        PlayerHUD.enabled = false;
    }
}
