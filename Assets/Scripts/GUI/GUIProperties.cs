using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class GUIProperties : MonoBehaviour
{
    //Main panel and animation for activation/deactivation
    
    public GameObject panelGeneral;
    public GameObject panelVisualisation;
    public GameObject panelColor;

    public GameObject panelMain;
    Animator panelAnimation;
    bool isGUIActive = true;



    #region Reference scripts

    //Controller scripts
    public ParticleEffects particleScript;
    public LineInstantiatior lineScript;

    //Main visualisation script
    VisualisationMain visualisationScript;
    #endregion

    #region UI Property objects
    //Use average for smoothing values
    Toggle useAverageObj;
    bool useAverage;

    //Rising or falling particles
    Toggle risingObj;
    bool isRising;

    TextMeshProUGUI lineNumText;
    Slider lineNumSlider;
    //TODO probably not needed
    DropDownToEnum dropDownSpectrumScaling;

    TMP_Dropdown visualisationTypeDropdown;
    TMP_Dropdown fftSizeDropdown;

    public InputField hexColor;
    Toggle randomizeColors;
    bool randomColorBool;
    #endregion


    string previousHexColor;

    public bool UseAverage { get => useAverage; set => useAverage = value; }


    // Start is called before the first frame update
    void Start()
    {
        panelAnimation = panelMain.GetComponent<Animator>();
        lineScript = GameObject.Find("MainVisualisation").GetComponent<LineInstantiatior>();
        visualisationScript = GameObject.Find("MainVisualisation").GetComponent<VisualisationMain>();
        
        //Is rising or falling
        risingObj = GameObject.Find("Orientation").GetComponent<Toggle>();
        isRising = risingObj.isOn;
        risingObj.onValueChanged.AddListener(delegate { UpdateOrientation(); });

        //Randomize particle colors
        randomizeColors = GameObject.Find("ToggleRandomizeDominantColor").GetComponent<Toggle>();
        randomColorBool = randomizeColors.isOn;
        randomizeColors.onValueChanged.AddListener(delegate { ColorChange(hexColor, randomColorBool); });


        //Use average
        useAverageObj = GameObject.Find("UseAverage").GetComponent<Toggle>();
        UseAverage = useAverageObj.isOn;
        useAverageObj.onValueChanged.AddListener(delegate { UpdateValues(); });
        
        //Visualisation Type
        dropDownSpectrumScaling = GameObject.Find("ScalingStrategy").GetComponent<DropDownToEnum>();
        visualisationTypeDropdown = GameObject.Find("VisualisationTypeDropdown").GetComponent<TMP_Dropdown>();
        visualisationTypeDropdown.onValueChanged.AddListener(delegate { ChangeVisualisationType(visualisationTypeDropdown); });

        //FFT Size
        fftSizeDropdown = GameObject.Find("FftSizeDropdown").GetComponent<TMP_Dropdown>();
        fftSizeDropdown.onValueChanged.AddListener(delegate { UpdateValues(); });


        //Line number
        //lineNumInput = GameObject.Find("LineNumberInput").GetComponent<TMP_InputField>();
        lineNumSlider = GameObject.Find("LineNumberSlider").GetComponent<Slider>();
        lineNumSlider.value = visualisationScript.lineInstator.lineNum;
        lineNumSlider.onValueChanged.AddListener(delegate { ChangeLineNumber(); });

        lineNumText = GameObject.Find("BarNumberText").GetComponent<TextMeshProUGUI>();
        lineNumText.text = visualisationScript.barNumber.ToString();
        //lineNumInput.text = visualisationScript.lineInstator.lineNum.ToString();

        //Add listener to color for particles
        hexColor.onValueChanged.AddListener(delegate { ColorChange(hexColor, randomColorBool); });
        previousHexColor = hexColor.text;

        ChangeActivePanel("General");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(isGUIActive)
            {
                panelAnimation.Play("Deactivate");
            } else
            {
                panelAnimation.Play("Activate");
            }
            isGUIActive = !isGUIActive;
        }

        //Needed for color change since its changed by other element
        if(previousHexColor != hexColor.text)
        {
            ColorChange(hexColor, randomColorBool);
        }
        previousHexColor = hexColor.text;
    }

    void DropdownValueChanged(Toggle change)
    {
        UpdateValues();
    }

    void ColorChange(InputField colorName, bool useRandomColor)
    {
        randomColorBool = randomizeColors.isOn;
        if (particleScript.isActiveAndEnabled)
            particleScript.ChangeColor(colorName.text, randomColorBool);
        if(lineScript.isActiveAndEnabled)
            lineScript.ChangeColor(colorName.text);
    }

    public void UpdateValues()
    {
        ScalingStrategy x = (ScalingStrategy)dropDownSpectrumScaling.selection;

        visualisationScript.UpdateValues(x, UseAverage, fftSizeDropdown.options[fftSizeDropdown.value].text);
    }

    void ChangeVisualisationType(TMP_Dropdown visType)
    {
        particleScript.ChangeVisualisationMode(visType.options[visType.value].text);
    }

    void UpdateOrientation()
    {
        isRising = risingObj.isOn;
        particleScript.ChangeOrientation();
    }

    void ChangeLineNumber()
    {
        visualisationScript.barNumber = (int)lineNumSlider.value;
        lineNumText.text = lineNumSlider.value.ToString();
        UpdateValues();
    }

    //ButtonMethods
    public void ChangeActivePanel(string activePanel)
    {
        if(activePanel == "General") {
            panelGeneral.SetActive(true);
            panelVisualisation.SetActive(false);
            panelColor.SetActive(false);
        } else if(activePanel == "Visualisation")
        {
            panelGeneral.SetActive(false);
            panelVisualisation.SetActive(true);
            panelColor.SetActive(false);
        } else if (activePanel == "Color")
        {
            panelGeneral.SetActive(false);
            panelVisualisation.SetActive(false);
            panelColor.SetActive(true);
        }
    }
}
