using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CSCore.DSP;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

using UnityEngine.SceneManagement;

public class VisualisationMain : MonoBehaviour
{
    AudioCapture audioCapture;
    [HideInInspector]
    public LineInstantiatior lineScript;
    [HideInInspector]
    public ParticleEffects particleScript;

    # region PUBLIC PROPERTIES
    public int barNumber = 64;
    public FftSize fftSize = FftSize.Fft2048;
    public int minFreq = 20;
    public int maxFreq = 4186;
    // float value for spectrum key 
    public float[] resData;
    public ScalingStrategy scalingStrategy;

    public float maxHeight;

    //Smooths the line values
    public bool useAverage;

    public PostProcessProfile postProcessObj;
    public float bloomIntensity;

    public Color barColor;
    public bool lineActive = true;


    #endregion
    // Start is called before the first frame update
    void Start()
    {
        audioCapture = gameObject.GetComponent<AudioCapture>();

        lineScript = gameObject.GetComponent<LineInstantiatior>();
        lineScript.lineNum = barNumber;

        particleScript = gameObject.GetComponent<ParticleEffects>();
        particleScript.lineNum = barNumber;

        if (!lineActive)
        {
            particleScript.InstantiateVisualisation();
        }
        if (lineActive)
        {
            lineScript.InstantiateVisualisation();
        }
        
        audioCapture.Initialize(fftSize, minFreq, maxFreq, scalingStrategy, useAverage, barNumber);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        float[] res = audioCapture.GetFFtData(maxHeight);

        if (res.Length == barNumber)
        {
            if (lineActive && lineScript.lineNum == barNumber)
            {
                lineScript.MapFrequencies(res);
            }
            if (!lineActive && particleScript.lineNum == barNumber)
            {
                particleScript.MapFrequencies(res);
            }
        }
    }



    private void OnApplicationQuit()
    {
        audioCapture.FreeData();
    }

    public void ReinstantiateVisualisation(string scaling, bool useAverage, string fftSize)
    {
        audioCapture.resolutionSize = barNumber;
        this.fftSize = (FftSize)System.Enum.Parse(typeof(FftSize), fftSize);
        this.scalingStrategy = (ScalingStrategy)System.Enum.Parse(typeof(ScalingStrategy), scaling);
        audioCapture.Initialize(this.fftSize, minFreq, maxFreq, this.scalingStrategy, useAverage, barNumber);
        if (lineActive)
        {
            lineScript.lineNum = barNumber;
            lineScript.InstantiateVisualisation();
        }
        if (!lineActive)
        {
            particleScript.lineNum = barNumber;
            particleScript.InstantiateVisualisation();
        }
    }

    public void ChangeBloomIntensity()
    {
        postProcessObj.TryGetSettings<Bloom>(out var bloom);
        bloom.intensity.overrideState = true;
        bloom.intensity.value = bloomIntensity;
    }

    public void ChangeColor(Color colorName)
    {
        barColor = colorName;
        if (!lineActive)
            particleScript.ChangeColor(barColor);
        if (lineActive)
            lineScript.ChangeColor(barColor);
    }
}
