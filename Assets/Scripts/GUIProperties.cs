using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class GUIProperties : MonoBehaviour
{
    //Main panel and animation for activation/deactivation
    public GameObject panel;
    Animator panelAnimation;
    bool isGUIActive = true;



    #region Reference scripts

    //Particle Controller script
    public ParticleEffects particleScript;

    //Main visualisation script
    VisualisationMain visualisation;
    #endregion

    #region UI Property objects
    //Use average for smoothing values
    Toggle useAverageObj;
    TMP_InputField lineNumInput;
    //TODO probably not needed
    DropDownToEnum dropDownSpectrumScaling;

    TMP_Dropdown visualisationTypeDropdown;

    public InputField hexColor;
    #endregion


    string previousHexColor;
    bool useAverage;

    public bool UseAverage { get => useAverage; set => useAverage = value; }


    // Start is called before the first frame update
    void Start()
    {
        panelAnimation = panel.GetComponent<Animator>();
        useAverageObj = panel.transform.Find("UseAverage").GetComponent<Toggle>();
        UseAverage = useAverageObj.isOn;
        dropDownSpectrumScaling = panel.transform.Find("ScalingStrategy").GetComponent<DropDownToEnum>();
        visualisationTypeDropdown = panel.transform.Find("VisualisationTypeDropdown").GetComponent<TMP_Dropdown>();

        visualisation = GameObject.Find("MainVisualisation").GetComponent<VisualisationMain>();
        lineNumInput = GameObject.Find("LineNumberInput").GetComponent<TMP_InputField>();
        lineNumInput.text = visualisation.lineInstator.lineNum.ToString();
        useAverageObj.onValueChanged.AddListener(delegate { UpdateValues(); });
        //Add listener to color for particles
        hexColor.onValueChanged.AddListener(delegate { ColorChange(hexColor); });
        previousHexColor = hexColor.text;
        visualisationTypeDropdown.onValueChanged.AddListener(delegate { ChangeVisualisationType(visualisationTypeDropdown); });

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
            ColorChange(hexColor);
        }
        previousHexColor = hexColor.text;

    }

    void DropdownValueChanged(Toggle change)
    {
        UpdateValues();
    }

    void ColorChange(InputField colorName)
    {
        Debug.Log("Changed color!");
        particleScript.ChangeColor(colorName.text);
    }

    public void UpdateValues()
    {
        ScalingStrategy x = (ScalingStrategy)dropDownSpectrumScaling.selection;
        visualisation.UpdateValues(x, UseAverage);
    }

    void ChangeVisualisationType(TMP_Dropdown visType)
    {
        Debug.Log( visType.options[visType.value].text);
        particleScript.ChangeVisualisationMode(visType.options[visType.value].text);
    }



}
