using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DropDownToEnum : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    GUIProperties guiProperties;


    public enum SpectrumScaling { Linear, Sqrt, Decibel }
    public SpectrumScaling selection;

    private void Awake()
    {
        dropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(dropdown); });//Add listener to Event
        guiProperties = GameObject.Find("Canvas").GetComponent<GUIProperties>();
    }

    void DropdownValueChanged(TMP_Dropdown change)
    {
        selection = (SpectrumScaling)change.value; //Convert dropwdown value to enum
        guiProperties.UpdateValues();
    }
}
