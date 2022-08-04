using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CSCore;
using CSCore.SoundIn;
using CSCore.DSP;
using CSCore.Streams;
using System;
//This is a custom class that imports FFT reading
//using WinformsVisualization.Visualization;


public class AudioCapture : MonoBehaviour
{
    //All objects needed to listen to audio
    #region Audio Capture

    //Captures audio stream
    WasapiCapture capture;


    //Interface for providing raw byte data of input audio
    IWaveSource finalSource;

    //Class for separating spectrum
    public SpectrumBase spectrumBase;

    //Reads a sequence of samples

    #endregion
    //-----------------------------------------------

    //Values that can be changed by user
    #region Public properties
    
    [HideInInspector]
    public FftSize fftSize;
    int minFreq;
    int maxFreq;
    // float value for spectrum key 
    ScalingStrategy scalingStrategy;

    public bool useAverage = true;

    #endregion
    //-----------------------------------------------

    public int resolutionSize;
    [HideInInspector]
    public float[] fftBuffer;

    public void Initialize(FftSize fftSize, int minFreq, int maxFreq, ScalingStrategy scalingStrategy, bool useAverage, int resolutionSize)
    {
        this.fftSize = fftSize;
        this.minFreq = minFreq;
        this.maxFreq = maxFreq;
        this.resolutionSize = resolutionSize;
        this.scalingStrategy = scalingStrategy;
        this.useAverage = useAverage;
        StartCapture();
    }

    public void StartCapture()
    {
        if (capture != null)
        {
            FreeData();
        }

        //Initialize the API for capturing audio from any source
        capture = new WasapiLoopbackCapture();
        capture.Initialize();

        //Implementation for IWaveSource - provides data specified by ISoundIn
        IWaveSource source = new SoundInSource(capture);

        fftBuffer = new float[(int)fftSize];

        //Initialize SpectrumBase class which will take audio and size of fft resolution

        spectrumBase = new SpectrumBase(source.WaveFormat.Channels, source.WaveFormat.SampleRate, fftSize, scalingStrategy, useAverage, resolutionSize);
        spectrumBase.MinFreq = minFreq;
        spectrumBase.MaxFreq = maxFreq;
        spectrumBase.UpdateFrequencyMapping();

        //Give samples instead of raw data from the stream and pass it to a new block
        SingleBlockNotificationStream notificationSource = new SingleBlockNotificationStream(source.ToSampleSource());

        notificationSource.SingleBlockRead += singleBlockNotificationStream_SingleBlockRead;
       
        finalSource = notificationSource.ToWaveSource();
        
        capture.DataAvailable += Capture_DataAvailable;
        capture.Start();
    }


    public void FreeData()
    {
        finalSource.Dispose();
        capture.Stop();
        capture.Dispose();
    }

    public float[] GetFFtData(float maxHeight)
    {
        spectrumBase.GetFftData(fftBuffer, this);
        return spectrumBase.GetSpectrumPoints(maxHeight, fftBuffer);
    }

    private void singleBlockNotificationStream_SingleBlockRead(object sender, SingleBlockReadEventArgs e)
    {
        spectrumBase.Add(e.Left, e.Right);
    }
    

    private void Capture_DataAvailable(object sender, DataAvailableEventArgs e)
    {
        finalSource.Read(e.Data, e.Offset, e.ByteCount);
    }

}

