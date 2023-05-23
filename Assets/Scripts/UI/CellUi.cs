using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;


public class CellUi : MonoBehaviour
{
    [HideInInspector]
    public RawImage Image;
    [HideInInspector]
    public LandStructure landStructure;

    private void Awake()
    {
        Image = GetComponent<RawImage>();
    }

    public void OnClick()
    {
        GridEditor.Instance.ChooseLandStructure(landStructure);
    }
}
