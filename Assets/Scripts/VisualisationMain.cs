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

    public float maxHeight;

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
        
        audioCapture.Initialize(fftSize, minFreq, maxFreq, scalingStrategy, useAverage, barNumber);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(1.0f / Time.deltaTime);
        if(Input.GetKeyDown(KeyCode.R))
        {
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        float[] res = audioCapture.GetFFtData(maxHeight);
        if (res.Length == barNumber)
        {
            if (lineInstator.isActiveAndEnabled && lineInstator.lineNum == barNumber)
            {
                lineInstator.MapFrequencies(res);
            }
            if (particleEffects.isActiveAndEnabled)
            {
                particleEffects.MapFrequencies(res);
            }
        }
    }



    private void OnApplicationQuit()
    {
        audioCapture.FreeData();
    }

    public void UpdateValues(string scaling, bool useAverage, string fftSize, bool isInverted)
    {
        //Debug.Log("Update " + useAverage + ' ' + scaling + ' ' + fftSize + " " + isInverted);
        audioCapture.resolutionSize = barNumber;
        this.fftSize = (FftSize)System.Enum.Parse(typeof(FftSize), fftSize);
        this.scalingStrategy = (ScalingStrategy)System.Enum.Parse(typeof(ScalingStrategy), scaling);
        audioCapture.Initialize(this.fftSize, minFreq, maxFreq, this.scalingStrategy, useAverage, barNumber);
        if (lineInstator.isActiveAndEnabled)
        {
            Debug.Log("This is wrong!");
            lineInstator.lineNum = barNumber;
            lineInstator.isInverted = isInverted;
            lineInstator.InstantiateVisualisation();
        }
        if (particleEffects.isActiveAndEnabled)
        {
            particleEffects.lineNum = barNumber;
            particleEffects.InstantiateVisualisation();
        }
    }
}
