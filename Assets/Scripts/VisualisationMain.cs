using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CSCore.DSP;

using UnityEngine.SceneManagement;

public class VisualisationMain : MonoBehaviour
{
    AudioCapture audioCapture;
    [HideInInspector]
    public LineInstantiatior lineInstator;

    public ParticleEffects particleEffects;

    # region PUBLIC PROPERTIES
    public FftSize fftSize = FftSize.Fft2048;
    public int minFreq = 5;
    public int maxFreq = 4500;
    // float value for spectrum key 
    public float[] resData;
    public ScalingStrategy scalingStrategy;

    //Smooths the line values
    public bool useAverage;


    #endregion
    // Start is called before the first frame update
    void Awake()
    {
        audioCapture = gameObject.GetComponent<AudioCapture>();
        lineInstator = gameObject.GetComponent<LineInstantiatior>();
        particleEffects = gameObject.GetComponent<ParticleEffects>();

        audioCapture.Construct(fftSize, minFreq, maxFreq, resData, scalingStrategy, useAverage, lineInstator.lineNum);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        float[] res = audioCapture.GetFFtData();
        if(lineInstator.isActiveAndEnabled)
            lineInstator.MapFrequencies(res);
        if (particleEffects.isActiveAndEnabled)
            if(particleEffects.transform.childCount == particleEffects.lineNum)
                particleEffects.MapFrequencies(res);
    }



    private void OnApplicationQuit()
    {
        audioCapture.FreeData();
    }

    public void UpdateValues(ScalingStrategy scaling, bool useAverage)
    {
        audioCapture.StartCapture();
        audioCapture.Construct(fftSize, minFreq, maxFreq, resData, scaling, useAverage, lineInstator.lineNum);
        audioCapture.spectrumBase.UpdateValues(scaling, useAverage);
    }
}
