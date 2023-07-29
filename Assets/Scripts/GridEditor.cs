using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Controllers;
using Managers;
using ScriptableObjects;
using Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class GridEditor : MonoBehaviour
{
    public static GridEditor Instance;
    public LandStructure landStructureToPlace;
    public LandScapeCell landScapeCellToPlace;
    public GridCell GridCellToChange => PlayerInteraction.SelectedCell;

    public int rotationTimes = 0;

    public GameObject structurePresentation;

    private RawImage _structurePresentationImage;


    public void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        _structurePresentationImage = structurePresentation.GetComponent<RawImage>();
        structurePresentation.SetActive(false);
    }

    public void ChooseLandStructure(LandStructure landStructure)
    {
        if (!landStructure.rotatable)
        {
            rotationTimes = -1;
        }
        else
        {
            rotationTimes = 0;
        }

        structurePresentation.SetActive(true);
        structurePresentation.transform.rotation = Quaternion.identity;
        landStructureToPlace = landStructure;
        landScapeCellToPlace = null;
        _structurePresentationImage.texture = landStructure.sprite.texture;
    }

    public void ChooseLandScape(LandScapeCell landScapeCell)
    {
        rotationTimes = -1;
        structurePresentation.SetActive(true);
        structurePresentation.transform.rotation = Quaternion.identity;
        landStructureToPlace = null;
        landScapeCellToPlace = landScapeCell;
        _structurePresentationImage.texture = landScapeCell.sprite.texture;
    }

    public void DestroyStructure()
    {
        if (PlayerInteraction.SelectedCell)
        {
            PlayerInteraction.SelectedCell.RemoveStructure();
        }

        if (PlayerInteraction.SelectedArea != null)
        {
            foreach (var gridCell in PlayerInteraction.SelectedArea)
            {
                gridCell.RemoveStructure();
            }
        }
    }

    public void AddRotationRight()
    {
        rotationTimes++;
        structurePresentation.transform.Rotate(0, 0, 90);
    }

    public void SaveGrid()
    {
        var path = OpenFileHelper.GetPathToSaveJsonFile();

        if (path == "")
        {
            Debug.Log("You have to choose a path to save a grid");
            return;
        }

        var json = GridManager.MapToJson(GridManager.Instance.gridCells);

        //Save to file
        System.IO.File.WriteAllText(path, json);
    }

    public void LoadGridFromJson(string path)
    {
        if (path == "")
        {
            path = OpenFileHelper.GetPathToLoadJsonFile();
        }

        if (path == "")
        {
            Debug.Log("You have to choose a path to load a grid");
            return;
        }

        //Load from file
        string json = System.IO.File.ReadAllText(path);

        //Deserialize JSON to grid
        var serializedGridCells = JsonHelper.FromJson<GridCellSerialization>(json);

        if (serializedGridCells.Length == 0)
        {
            Debug.Log("Grid is empty");
            return;
        }

        //If grid already exists destroy it
        if (GridManager.Instance.gridCells.Count > 0)
        {
            DestroyGrid();
        }

        foreach (var serializedGridCell in serializedGridCells)
        {
            if (GridManager.Instance.gridCells.Count() == GridManager.GridBoundX * GridManager.GridBoundY)
            {
                Debug.Log("Grid is full");
                break;
            }

            serializedGridCell.CreateGridCell();
        }

        GridManager.Instance.GridSuccessfullyGenerated();
    }

    public void LoadGridFromImage()
    {
        var path = OpenFileHelper.GetPathToLoadImageFile();

        if (path == "")
        {
            Debug.Log("You have to choose a path to load a grid");
            return;
        }

        //Set mapSprite in GridManager to loaded image
        GridManager.Instance.mapTexture = Texture2DHelper.LoadImage(path);

        if (GridManager.Instance.mapTexture == null)
        {
            Debug.LogError("Sprite load fail");
            return;
        }

        if (GridManager.Instance.mapTexture.GetRawTextureData() == null)
        {
            Debug.LogError("Texture load fail");
            return;
        }

        //If grid already exists destroy it
        if (GridManager.Instance.gridCells.Count > 0)
        {
            DestroyGrid();
        }

        //Generate grid from mapSprite
        GridManager.Instance.GenerateFromImage();
    }

    public void DestroyGrid()
    {
        var gridCells = GridManager.Instance.gridCells;
        //destroy gameobjects
        foreach (var gridCell in gridCells)
        {
            Destroy(gridCell.gameObject);
        }

        //clear list
        gridCells.Clear();

        Debug.Log("Grid destroyed");
    }

    public void PlaceStructure()
    {
        if (landStructureToPlace != null)
        {
            if (!PlayerInteraction.SelectedCell)
            {
                Debug.Log("You have to choose a cell to place a structure");
                return;
            }

            PlayerInteraction.SelectedCell.SetStructure(landStructureToPlace);

            if (PlayerInteraction.SelectedArea.Count > 0)
            {
                foreach (var cell in PlayerInteraction.SelectedArea)
                {
                    cell.SetStructure(landStructureToPlace);
                }
            }
        }
        else if (landScapeCellToPlace != null)
        {
            if (!PlayerInteraction.SelectedCell)
            {
                Debug.Log("You have to choose a cell to change its land type");
                return;
            }

            PlayerInteraction.SelectedCell.SetLandType(landScapeCellToPlace);

            if (PlayerInteraction.SelectedArea.Count > 0)
            {
                foreach (var cell in PlayerInteraction.SelectedArea)
                {
                    cell.SetLandType(landScapeCellToPlace);
                }
            }
        }
    }
}