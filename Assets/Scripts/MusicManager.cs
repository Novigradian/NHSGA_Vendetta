using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioLowPassFilter lowPassFilter;

    [SerializeField] private float muffledCutOffFrequency;
    [SerializeField] private float originalCutOffFrequency;
    
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartMuffle()
    {
        lowPassFilter.cutoffFrequency = muffledCutOffFrequency;
    }

    public void EndMuffle()
    {
        lowPassFilter.cutoffFrequency = originalCutOffFrequency;
    }
}
