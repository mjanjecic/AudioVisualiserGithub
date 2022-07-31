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
    [HideInInspector]
    public ParticleEffects particleEffects;

    # region PUBLIC PROPERTIES
    public int barNumber = 64;
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
    void Start()
    {
        audioCapture = gameObject.GetComponent<AudioCapture>();
        lineInstator = gameObject.GetComponent<LineInstantiatior>();
        lineInstator.lineNum = barNumber;
        particleEffects = gameObject.GetComponent<ParticleEffects>();
        particleEffects.lineNum = barNumber;
        if (particleEffects.isActiveAndEnabled)
        {
            particleEffects.InstantiateVisualisation();
        }
        if (lineInstator.isActiveAndEnabled)
        {
            lineInstator.InstantiateVisualisation();
        }
        
        audioCapture.Construct(fftSize, minFreq, maxFreq, resData, scalingStrategy, useAverage, barNumber);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(1.0f / Time.deltaTime);
        if(Input.GetKeyDown(KeyCode.R))
        {
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        float[] res = audioCapture.GetFFtData();
        if(lineInstator.isActiveAndEnabled)
            lineInstator.MapFrequencies(res);
        if (particleEffects.isActiveAndEnabled)
            particleEffects.MapFrequencies(res);
            //if(particleEffects.transform.childCount == particleEffects.lineNum)
    }



    private void OnApplicationQuit()
    {
        audioCapture.FreeData();
    }

    public void UpdateValues(ScalingStrategy scaling, bool useAverage, string fftSize)
    {
        if (lineInstator.isActiveAndEnabled)
        {

            lineInstator.lineNum = barNumber;
            lineInstator.InstantiateVisualisation();
        }
        if (particleEffects.isActiveAndEnabled)
        {
            particleEffects.lineNum = barNumber;
            particleEffects.InstantiateVisualisation();
        }
        audioCapture.resolutionSize = barNumber;
        audioCapture.StartCapture();
        this.fftSize = (FftSize)System.Enum.Parse(typeof(FftSize), fftSize);
        audioCapture.Construct(this.fftSize, minFreq, maxFreq, resData, scaling, useAverage, barNumber);
        audioCapture.spectrumBase.UpdateValues(scaling, useAverage);
    }
}
