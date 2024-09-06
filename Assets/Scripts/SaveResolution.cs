using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Dropdown))]
public class SaveResolution : MonoBehaviour
{
    public TMP_Dropdown ResDropDown;

    const string PrefName = "optionValue";
    void Awake()
    {
        ResDropDown = GetComponent<TMP_Dropdown>();
        ResDropDown.onValueChanged.AddListener(new UnityAction<int>(index =>
        {
            PlayerPrefs.SetInt(PrefName, ResDropDown.value);
            PlayerPrefs.Save();
        }));
    }
    // Start is called before the first frame update
    void Start()
    {
        ResDropDown.value = PlayerPrefs.GetInt(PrefName, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
