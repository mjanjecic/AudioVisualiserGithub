using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class GUIPropertiesBase : MonoBehaviour
{

    //Controller scripts
    public ParticleEffects particleScript;
    public LineInstantiatior lineScript;
    public VisualisationMain visualisationScript;

    //Dropdowns
    public TMP_Dropdown dropDownSpectrumScaling;
    public TMP_Dropdown visualisationTypeDropdown;
    public TMP_Dropdown visualisationObjectsDropdown;
    public TMP_Dropdown fftSizeDropdown;

    //Toggles
    //Use average for smoothing values
    public Toggle useAverageObj;

    //Sliders
    //Line numbers
    public TextMeshProUGUI lineNumText;
    public Slider lineNumSlider;
    //Max height/value scaling
    public TextMeshProUGUI maxHeightText;
    public Slider maxHeightSlider;

    //Bloom intensity
    public Slider bloomSlider;
    public TextMeshProUGUI bloomValueText;

    // Start is called before the first frame update
    void Awake()
    {
        lineScript = GameObject.Find("MainVisualisation").GetComponent<LineInstantiatior>();
        visualisationScript = GameObject.Find("MainVisualisation").GetComponent<VisualisationMain>();
        particleScript = GameObject.Find("MainVisualisation").GetComponent<ParticleEffects>();

        dropDownSpectrumScaling = GameObject.Find("ScalingStrategy").GetComponent<TMP_Dropdown>();
        visualisationTypeDropdown = GameObject.Find("DropdownVisualisationShape").GetComponent<TMP_Dropdown>();

        //Visualisation shape - particles or lines
        visualisationObjectsDropdown = GameObject.Find("DropdownVisualisationObjects").GetComponent<TMP_Dropdown>();

        //FFT Size
        fftSizeDropdown = GameObject.Find("FftSizeDropdown").GetComponent<TMP_Dropdown>();

        //Use average
        useAverageObj = GameObject.Find("UseAverage").GetComponent<Toggle>();

        //Line number
        lineNumSlider = GameObject.Find("LineNumberSlider").GetComponent<Slider>();
        lineNumSlider.value = visualisationScript.barNumber;

        lineNumText = GameObject.Find("BarNumberText").GetComponent<TextMeshProUGUI>();
        lineNumText.text = visualisationScript.barNumber.ToString();

        //Height/strength multiplier
        maxHeightSlider = GameObject.Find("MaxHeightSlider").GetComponent<Slider>();
        maxHeightSlider.value = visualisationScript.barNumber;

        maxHeightText = GameObject.Find("MaxHeightText").GetComponent<TextMeshProUGUI>();
        maxHeightText.text = visualisationScript.maxHeight.ToString();

        //Bloom intensity
        bloomSlider = GameObject.Find("BloomStrengthSlider").GetComponent<Slider>();
        bloomSlider.value = 0;
        bloomValueText = GameObject.Find("BloomIntensityText").GetComponent<TextMeshProUGUI>();
        bloomValueText.text = bloomSlider.value.ToString();
    }
}
