using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightFirstButton : MonoBehaviour
{
    Button button;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.Select();
        }
        else
        {
            Debug.LogError("Button component not found on this GameObject.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
