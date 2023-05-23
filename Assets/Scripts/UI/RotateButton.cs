using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateButton : MonoBehaviour
{
    private Button _button;

    public void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Update()
    {
        if (GridEditor.Instance.rotationTimes == -1)
        {
            _button.interactable = false;
        }
        else
        {
            _button.interactable = true;
        }
    }
}
