using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CSCore.DSP;
using System;
using CSCore;
using System.Linq;

public class SpectrumBase : FftProvider
{
    int sampleRate;
    private readonly List<object> _contexts = new List<object>();

    //FrequencyMapping
    private const int ScaleFactorLinear = 90;
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

    ScalingStrategy scalingStrategy;
    bool useAverage;

    int spectrumSize;

    public int MinFreq { get => _minimumFrequency; set => _minimumFrequency = value; }
    public int MaxFreq { get => _maximumFrequency; set => _maximumFrequency = value; }


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
        _maxFftIndex = (int)fftSize/2-1;
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
        _maximumFrequencyIndex = Math.Min(GetFftBandIndex(_maximumFrequency) + 1, _maxFftIndex);
        _minimumFrequencyIndex = Math.Min(GetFftBandIndex(_minimumFrequency), _maxFftIndex);
        

        int actualResolution = spectrumSize;
        int indexCount = _maximumFrequencyIndex - _minimumFrequencyIndex;
        double linearIndexBucketSize = Math.Round(indexCount / (double)actualResolution, 3);

        _spectrumIndexMax = _spectrumIndexMax.CheckBuffer(actualResolution, true);
        _spectrumLogScaleIndexMax = _spectrumLogScaleIndexMax.CheckBuffer(actualResolution, true);

        double maxLog = Math.Log(actualResolution, actualResolution);

        //MyTest
        double multiplier = Math.Log(indexCount, actualResolution);

        for (int i = 1; i < actualResolution; i++)
        {
            int freqRangeMax = (int)Math.Ceiling(Math.Pow((i + 1), multiplier)) + _minimumFrequencyIndex - 1;
            Debug.Log(freqRangeMax);
            int logIndex =
                (int)((maxLog - Math.Log((actualResolution + 1) - i, (actualResolution + 1))) * indexCount) +
                _minimumFrequencyIndex;


            //_spectrumIndexMax[i - 1] = _minimumFrequencyIndex + (int)(i * linearIndexBucketSize);
            _spectrumIndexMax[i - 1] = _minimumFrequencyIndex + (int)(i * linearIndexBucketSize);
            //_spectrumLogScaleIndexMax[i - 1] = logIndex;
            _spectrumLogScaleIndexMax[i - 1] = logIndex;
        }

        if (actualResolution > 0)
        {
            _spectrumIndexMax[_spectrumIndexMax.Length - 1] = _spectrumLogScaleIndexMax[_spectrumLogScaleIndexMax.Length - 1] = _maximumFrequencyIndex;
        }
        for (int i = 1; i < actualResolution; i++)
        {

        //Debug.Log(_spectrumIndexMax[i-1]);
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

        double average = 0.0;
        int averageNumber = 0;

        for (int i = _minimumFrequencyIndex; i <= _maximumFrequencyIndex; i++)
        {
            //value0 = FastFourierTransformation.HammingWindow(i, (int)FftSize);
            switch (scalingStrategy)
            {
                case ScalingStrategy.Decibel:
                    value0 = (((20 * Math.Log10(fftBuffer[i])) - MinDbValue) / DbScale) * actualMaxValue;
                    break;
                case ScalingStrategy.Linear:
                    value0 = (fftBuffer[i] * ScaleFactorLinear) * actualMaxValue ;
                    break;
                case ScalingStrategy.Sqrt:
                    value0 = ((Math.Sqrt(fftBuffer[i]))) * actualMaxValue;
                    break;


            }

            //Use hamming window function
            //value0 *= FastFourierTransformation.HammingWindow(i, (int)FftSize-1);
            value0 *= FastFourierTransformation.HammingWindow(i, _maximumFrequencyIndex);


            //lastValue = value0;
            average += value0;
            averageNumber++;



            bool recalc = true;
            //value = Math.Max(0, Math.Max(value0, value));
            average += value0;
            averageNumber++;
            value = Math.Max(value0, value);

            if (spectrumPointIndex < _spectrumIndexMax.Length &&
                   i == _spectrumLogScaleIndexMax[spectrumPointIndex])
            {
                //if (!recalc)
                //    value = lastValue;

                
                //if (useAverage && spectrumPointIndex > 0)
                //    value = (lastValue + value) / 2;
                if (value > maxValue)
                    value = maxValue;
                if (useAverage)
                {
                    value0 = (average + 0.0) / averageNumber;
                }
                dataPoints.Add(new SpectrumPointData { SpectrumPointIndex = spectrumPointIndex, Value = value0 });
                averageNumber = 0;
                average = 0;
                //lastValue = value;
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
