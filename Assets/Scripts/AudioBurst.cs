using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBurst : MonoBehaviour
{
    public AudioSource AudioSource;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!AudioSource.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
