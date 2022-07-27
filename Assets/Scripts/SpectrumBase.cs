using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CSCore.DSP;
using System;
using CSCore;

public class SpectrumBase : FftProvider
{
    int sampleRate;
    private readonly List<object> _contexts = new List<object>();

    //FrequencyMapping
    private const int ScaleFactorLinear = 9;
    protected const int ScaleFactorSqr = 2;
    protected const double MinDbValue = -90;
    protected const double MaxDbValue = 0;
    protected const double DbScale = (MaxDbValue - MinDbValue);

    private int _maxFftIndex;
    private int _maximumFrequency = 20000;
    private int _maximumFrequencyIndex;
    private int _minimumFrequency = 50; //Default spectrum from 20Hz to 20kHz
    private int _minimumFrequencyIndex;
    private int[] _spectrumIndexMax;
    private int[] _spectrumLogScaleIndexMax;


    int minFreq = 5;
    int maxFreq = 4500;

    ScalingStrategy scalingStrategy;
    bool useAverage;

    int spectrumSize;

    public int MinFreq { get => minFreq; set => minFreq = value; }
    public int MaxFreq { get => maxFreq; set => maxFreq = value; }


    //Constructor
    public SpectrumBase(int channels,int sampleRate, FftSize fftSize, ScalingStrategy scalingStrategy, bool useAverage, int spectrumSize)  
        : base (channels, fftSize)
    {
        if (sampleRate <= 0)
            throw new ArgumentOutOfRangeException("sampleRate");
        this.sampleRate = sampleRate;
        this.scalingStrategy = scalingStrategy;
        this.useAverage = useAverage;
        this.spectrumSize = spectrumSize;
        _maxFftIndex = (int)fftSize / 2 - 1;
    }

    public new void Add(float left, float right)
    {
        base.Add(left, right);
        _contexts.Clear();
    }


    public int GetFftBandIndex(float frequency)
    {
        int fftSize = (int)FftSize;
        
        double f = sampleRate / 2.0;
        // ReSharper disable once PossibleLossOfFraction
        return (int)((frequency / f) * (fftSize / 2));
    }

    public void UpdateValues(ScalingStrategy scaling, bool useAverage)
    {
        scalingStrategy = scaling;
        this.useAverage = useAverage;
        UpdateFrequencyMapping();
    }

    public void UpdateFrequencyMapping()
    {
        //_maximumFrequencyIndex = Math.Min(GetFftBandIndex(_maximumFrequency) + 1, _maxFftIndex);
        //_minimumFrequencyIndex = Math.Min(GetFftBandIndex(_minimumFrequency), _maxFftIndex);
        _maximumFrequencyIndex = MaxFreq;
        _minimumFrequencyIndex = MinFreq;

        int actualResolution = spectrumSize;
        int indexCount = _maximumFrequencyIndex - _minimumFrequencyIndex;
        double linearIndexBucketSize = Math.Round(indexCount / (double)actualResolution, 3);

        _spectrumIndexMax = _spectrumIndexMax.CheckBuffer(actualResolution, true);
        _spectrumLogScaleIndexMax = _spectrumLogScaleIndexMax.CheckBuffer(actualResolution, true);

        double maxLog = Math.Log(actualResolution, actualResolution);
        for (int i = 1; i < actualResolution; i++)
        {
            int logIndex =
                (int)((maxLog - Math.Log((actualResolution + 1) - i, (actualResolution + 1))) * indexCount) +
                _minimumFrequencyIndex;

            _spectrumIndexMax[i - 1] = _minimumFrequencyIndex + (int)(i * linearIndexBucketSize);
            _spectrumLogScaleIndexMax[i - 1] = logIndex;
        }

        if (actualResolution > 0)
        {
            _spectrumIndexMax[_spectrumIndexMax.Length - 1] = _spectrumLogScaleIndexMax[_spectrumLogScaleIndexMax.Length - 1] = _maximumFrequencyIndex;
        }
    }


    public bool GetFftData(float[] fftResultBuffer, object context)
    {
        if (_contexts.Contains(context))
            return false;

        _contexts.Add(context);
        GetFftData(fftResultBuffer);
        return true;
    }


    public virtual SpectrumPointData[] CalculateSpectrumPoints(double maxValue, float[] fftBuffer)
    {
        var dataPoints = new List<SpectrumPointData>();

        double value0 = 0, value = 0;
        double lastValue = 0;
        double actualMaxValue = maxValue;
        int spectrumPointIndex = 0;


        for (int i = _minimumFrequencyIndex; i <= _maximumFrequencyIndex; i++)
        {
            switch (scalingStrategy)
            {
                case ScalingStrategy.Decibel:
                    value0 = (((20 * Math.Log10(fftBuffer[i])) - MinDbValue) / DbScale) * actualMaxValue;
                    break;
                case ScalingStrategy.Linear:
                    value0 = (fftBuffer[i] * ScaleFactorLinear) * actualMaxValue;
                    break;
                case ScalingStrategy.Sqrt:
                    value0 = ((Math.Sqrt(fftBuffer[i])) * ScaleFactorSqr) * actualMaxValue;
                    break;


            }

            bool recalc = true;

            value = Math.Max(0, Math.Max(value0, value));

            /*
             
                while (spectrumPointIndex <= _spectrumIndexMax.Length - 1 &&
                       i ==
                       (IsXLogScale
                           ? _spectrumLogScaleIndexMax[spectrumPointIndex]
                           : _spectrumIndexMax[spectrumPointIndex]))*/

            while (spectrumPointIndex <= _spectrumIndexMax.Length - 1 &&
                   i == _spectrumLogScaleIndexMax[spectrumPointIndex])
            {
                if (!recalc)
                    value = lastValue;

                if (value > maxValue)
                    value = maxValue;

                if (useAverage && spectrumPointIndex > 0)
                    value = (lastValue + value) / 2;


                dataPoints.Add(new SpectrumPointData { SpectrumPointIndex = spectrumPointIndex, Value = value });

                lastValue = value;
                value = 0.0;
                spectrumPointIndex++;
                recalc = false;
            }

            //value = 0;
        }

        return dataPoints.ToArray();
    }

    public struct SpectrumPointData
    {
        public int SpectrumPointIndex;
        public double Value;
    }

    public float[] GetSpectrumPoints(float height, float[] fftBuffer)
    {
        SpectrumPointData[] dats = CalculateSpectrumPoints(height, fftBuffer);
        float[] res = new float[dats.Length];
        for (int i = 0; i < dats.Length; i++)
        {
            res[i] = (float)dats[i].Value;
        }

        return res;
    }
}
