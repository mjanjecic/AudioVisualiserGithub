using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

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

    public InputField hexColor;
    #endregion


    string previousHexColor;

    public bool UseAverage { get => useAverage; set => useAverage = value; }


    // Start is called before the first frame update
    void Start()
    {
        panelAnimation = panelMain.GetComponent<Animator>();

        //Use average
        UseAverage = useAverageObj.isOn;
        useAverageObj.onValueChanged.AddListener(delegate { UpdateValues(); });

        //Visualisation Type - linear or circular
        visualisationTypeDropdown.onValueChanged.AddListener(delegate { ChangeVisualisationType(); });
        
        //Visualisation shape - particles or lines
        visualisationObjectsDropdown.onValueChanged.AddListener(delegate { ChangeVisualisationType(); });

        //FFT Size
        fftSizeDropdown.onValueChanged.AddListener(delegate { UpdateValues(); });

        //Spectrum scaling
        dropDownSpectrumScaling.onValueChanged.AddListener(delegate { UpdateValues(); });

        //Line number slider
        lineNumSlider.onValueChanged.AddListener(delegate { ChangeLineNumber(); });

        //Max height slider
        maxHeightSlider.onValueChanged.AddListener(delegate { ChangeMaxHeight(); });

        //Add listener to color for particles
        hexColor.onValueChanged.AddListener(delegate { ColorChange(hexColor.text); });
        previousHexColor = hexColor.text;

        //Add listener for bloom intensity
        bloomSlider.onValueChanged.AddListener(delegate { ChangeBloom(); });

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
            ColorChange(hexColor.text);
        }
        previousHexColor = hexColor.text;
    }


    void ColorChange(string colorName)
    {
        Color newCol;
        if (ColorUtility.TryParseHtmlString(colorName, out newCol))
        {
            visualisationScript.ChangeColor(newCol);
        }
    }


    public void UpdateValues()
    {
        UseAverage = useAverageObj.isOn;
        visualisationScript.ReinstantiateVisualisation(dropDownSpectrumScaling.options[dropDownSpectrumScaling.value].text, UseAverage, fftSizeDropdown.options[fftSizeDropdown.value].text);
    }

    public void ChangeMaxHeight()
    {
        visualisationScript.maxHeight = maxHeightSlider.value;
        maxHeightText.text = visualisationScript.maxHeight.ToString();
    }


    void ChangeVisualisationType()
    {
        particleScript.lineNum = lineScript.lineNum = (int)lineNumSlider.value;
        if (visualisationObjectsDropdown.options[visualisationObjectsDropdown.value].text == "Particles")
        {
            visualisationScript.lineActive = false;
            particleScript.ChangeVisualisationMode(visualisationTypeDropdown.options[visualisationTypeDropdown.value].text);
            var lineParent = visualisationScript.transform.Find("LineObjectParent");
            if (lineParent != null)
                Destroy(lineParent.gameObject);
        }
        else
        {
            visualisationScript.lineActive = true;
            lineScript.ChangeVisualisationMode(visualisationTypeDropdown.options[visualisationTypeDropdown.value].text);
            var particleParent = visualisationScript.transform.Find("ParticleObjectParent");
            if (particleParent != null)
                Destroy(particleParent.gameObject);
        }
        ColorChange(hexColor.text);
    }


    void ChangeLineNumber()
    {
        visualisationScript.barNumber = (int)lineNumSlider.value;
        lineNumText.text = lineNumSlider.value.ToString();
        UpdateValues();
    }

    void ChangeBloom()
    {
        visualisationScript.bloomIntensity = bloomSlider.value;
        bloomValueText.text = Math.Round(bloomSlider.value, 2).ToString();
        visualisationScript.ChangeBloomIntensity();
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
