using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class GUIProperties : GUIPropertiesBase
{
    //Main panel and animation for activation/deactivation
    
    public GameObject panelGeneral;
    public GameObject panelVisualisation;
    public GameObject panelColor;

    public GameObject panelMain;
    Animator panelAnimation;
    bool isGUIActive = true;

    #region UI Property objects
    bool useAverage;
    bool isRising;
    bool isInverted;

    public InputField hexColor;
    bool randomColorBool;
    #endregion


    string previousHexColor;

    public bool UseAverage { get => useAverage; set => useAverage = value; }


    // Start is called before the first frame update
    void Start()
    {
        panelAnimation = panelMain.GetComponent<Animator>();

        //Instantiate up or down 
        isRising = risingObj.isOn;
        risingObj.onValueChanged.AddListener(delegate { UpdateOrientation(); });

        //Randomize particle colors
        randomColorBool = randomizeColors.isOn;
        randomizeColors.onValueChanged.AddListener(delegate { ColorChange(hexColor, randomColorBool); });

        //Use average
        UseAverage = useAverageObj.isOn;
        useAverageObj.onValueChanged.AddListener(delegate { UpdateValues(); });

        //Invert spectrum
        isInverted = invertSpectrum.isOn;
        invertSpectrum.onValueChanged.AddListener(delegate { UpdateValues(); });

        //Visualisation Type
        visualisationTypeDropdown.onValueChanged.AddListener(delegate { ChangeVisualisationType(visualisationTypeDropdown); });
        
        //Visualisation shape - particles or lines
        visualisationObjectsDropdown.onValueChanged.AddListener(delegate { ChangeVisualisationType(visualisationTypeDropdown); });

        //FFT Size
        fftSizeDropdown.onValueChanged.AddListener(delegate { UpdateValues(); });

        //Spectrum scaling
        dropDownSpectrumScaling.onValueChanged.AddListener(delegate { UpdateValues(); });

        //Line number slider
        lineNumSlider.onValueChanged.AddListener(delegate { ChangeLineNumber(); });

        //Max height slider
        maxHeightSlider.onValueChanged.AddListener(delegate { ChangeMaxHeight(); });

        //Add listener to color for particles
        hexColor.onValueChanged.AddListener(delegate { ColorChange(hexColor, randomColorBool); });
        previousHexColor = hexColor.text;

        //Activate main panel - all have to be active at the beginning to get reference objects
        ChangeActivePanel("General");
    }


    private void Update()
    {
        //Hide or show configuration panel
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
        Debug.Log("Activated!");
        UseAverage = useAverageObj.isOn;
        isInverted = invertSpectrum.isOn;
        visualisationScript.UpdateValues(dropDownSpectrumScaling.options[dropDownSpectrumScaling.value].text, UseAverage, fftSizeDropdown.options[fftSizeDropdown.value].text, isInverted);
    }

    public void ChangeMaxHeight()
    {
        visualisationScript.maxHeight = maxHeightSlider.value;
        maxHeightText.text = visualisationScript.maxHeight.ToString();
    }


    void ChangeVisualisationType(TMP_Dropdown visType)
    {
        if (visualisationObjectsDropdown.options[visualisationObjectsDropdown.value].text == "Particles")
        {
            particleScript.enabled = true;
            lineScript.enabled = false;
            particleScript.lineNum = (int)lineNumSlider.value;
            particleScript.ChangeVisualisationMode(visType.options[visType.value].text);
            //var particleParent = visualisationScript.transform.Find("ParticleObjectParent");
            var lineParent = visualisationScript.transform.Find("LineObjectParent");
            if (lineParent != null)
                Destroy(lineParent.gameObject);
        }
        else
        {
            lineScript.enabled = true;
            particleScript.enabled = false;
            lineScript.lineNum = (int)lineNumSlider.value;
            lineScript.ChangeVisualisationMode(visType.options[visType.value].text);
            var particleParent = visualisationScript.transform.Find("ParticleObjectParent");
            if(particleParent != null)
            Destroy(particleParent.gameObject);
        }
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
