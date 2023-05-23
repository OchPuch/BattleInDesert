using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Controllers;
using Managers;
using TMPro;
using UnityEngine;


public class CellCostPopUp : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private Vector3 _worldPosition;
    private Camera _camera;
    private GridCell _attachedCell;
    CameraController _cameraController;

    [SerializeField] private float textSize = 30f;

    public float cost;

    private GridManager GridManager;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _camera = Camera.main;
        _cameraController = _camera.GetComponent<CameraController>();
        
        GridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
    }
    
    void Start()
    {
        _worldPosition = _camera.ScreenToWorldPoint(transform.position);
        _worldPosition.z = 0;
        _attachedCell = GridManager.GetGridCell(GridManager.PositionToGridPosition(_worldPosition));

        _text.text = (cost).ToString();
    }

    void Update()
    {
        var cellPosition = _attachedCell.transform.position;
        transform.position = _camera.WorldToScreenPoint(cellPosition);
        _text.fontSize = textSize * _cameraController.minZoom / _camera.orthographicSize;
    }
}