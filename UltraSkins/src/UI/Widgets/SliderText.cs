using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UltraSkins.UI.Widgets
{
    public class SliderText : MonoBehaviour
    {
        // Start is called before the first frame update
    public Slider slider;
    public TextMeshProUGUI valueText;

    void Start()
    {
        // Update once at start
        UpdateValueText(slider.value);

        // Subscribe to value changes
        slider.onValueChanged.AddListener(UpdateValueText);
    }

    void UpdateValueText(float value)
    {
        int normalized = Mathf.RoundToInt(value * 100f); // Convert to 0-100
        valueText.text = normalized.ToString();
    }
    }
}
