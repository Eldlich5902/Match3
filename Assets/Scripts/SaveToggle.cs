using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SaveToggle : MonoBehaviour
{
    public Toggle toggle;

    const string PrefName = "optionTick";

    // Start is called before the first frame update
    void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(new UnityAction<bool>(value =>
        {
            PlayerPrefs.SetInt(PrefName, value ? 1 : 0);
            PlayerPrefs.Save();
        }));
    }

    void Start()
    {
        toggle.isOn = PlayerPrefs.GetInt(PrefName, 0) == 1;
    }

    private void Update()
    {
        
    }
}
