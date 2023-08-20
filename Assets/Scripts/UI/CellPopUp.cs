using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;

public class CellPopUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    public GameObject attachedCell;
    
    public void SetText(string s)
    {
        text.text = s;
    }

    private void Start()
    {
        attachedCell = transform.parent.gameObject;
        transform.localScale = GridManager.CellSize;
    }
}
