using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Mirror;
using ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine.Serialization;

[System.Serializable]
public class GridCell : MonoBehaviour
{
    [FormerlySerializedAs("landScapeCell")] [SerializeField] public LandScapeCellSprites landScapeCellSprites;

    public Vector2Int gridPosition;
    public readonly GridCell[,] AdjacentCells = new GridCell[3, 3]; //Always check for nulls

    public UnitScript attachedUnit;
    public StructureScript attachedStructure;

    public bool Walkable = true;

    public HashSet<LandScapeCellSprites.LandType> landType = new HashSet<LandScapeCellSprites.LandType>();

    public float movementCost = 1f;



    //pathfinding
    public float gCost;
    public float hCost;
    public float fCost => gCost + hCost;

    [HideInInspector]
    public GridCell parent; // Used for pathfinding only

    [SerializeField] private SpriteRenderer spriteRenderer;

    public GridManager GridManager;
    
    private void Awake()
    {
        GridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        GridManager.GridGenerated += OnGridGenerated;
        var position = transform.position;
        gridPosition = new Vector2Int(Mathf.RoundToInt(position.x / GridManager.CellSize.x),
            Mathf.RoundToInt(position.y / GridManager.CellSize.y));

        GridManager.AddTile(gameObject);
    }

    private void Start()
    {
        
        if (!landScapeCellSprites) {
            Destroy(gameObject);
            return;
        }

        
        SetPositionByGridCoordinates();
        movementCost = landScapeCellSprites.movementCost;
        spriteRenderer.sprite = landScapeCellSprites.sprite;
        landType.Add(landScapeCellSprites.landType);
    }

    private void SetPositionByGridCoordinates()
    {
        transform.position = new Vector3(gridPosition.x * GridManager.CellSize.x,
            gridPosition.y * GridManager.CellSize.y, 0);
    }

    public bool IsWalkable(UnitScript walker)
    {
        if (!Walkable)
        {
            return false;
        }

        if (attachedUnit)
        {
            return false;
        }


        if (attachedStructure)
        {
            var inDrivers = false;
            foreach (var driver in attachedStructure.landStructure.canBeDrivenBy)
            {
                if (driver == walker.unit)
                {
                    inDrivers = true;
                    break;
                }
            }

            if (!inDrivers)
            {
                return false;
            }
        }

        var flag = false;

        foreach (var land in landType)
        {
            switch (land)
            {
                case LandScapeCellSprites.LandType.Water:
                    flag = walker.unit.canDriveWater;
                    break;
                case LandScapeCellSprites.LandType.Land:
                    flag = walker.unit.canDriveLand;
                    break;
                case LandScapeCellSprites.LandType.ShallowWater:
                    flag = walker.unit.canDriveShallowWater;
                    break;
            }
        }

        return flag;
    }

    private void OnGridGenerated()
    {
        AdjacentCells[1, 1] = null;
        AdjacentCells[0, 0] = GridManager.GetGridCell(new Vector2Int(gridPosition.x - 1, gridPosition.y - 1));
        AdjacentCells[0, 1] = GridManager.GetGridCell(new Vector2Int(gridPosition.x - 1, gridPosition.y));
        AdjacentCells[0, 2] = GridManager.GetGridCell(new Vector2Int(gridPosition.x - 1, gridPosition.y + 1));
        AdjacentCells[1, 0] = GridManager.GetGridCell(new Vector2Int(gridPosition.x, gridPosition.y - 1));
        AdjacentCells[1, 2] = GridManager.GetGridCell(new Vector2Int(gridPosition.x, gridPosition.y + 1));
        AdjacentCells[2, 0] = GridManager.GetGridCell(new Vector2Int(gridPosition.x + 1, gridPosition.y - 1));
        AdjacentCells[2, 1] = GridManager.GetGridCell(new Vector2Int(gridPosition.x + 1, gridPosition.y));
        AdjacentCells[2, 2] = GridManager.GetGridCell(new Vector2Int(gridPosition.x + 1, gridPosition.y + 1));
    }

    public GridCell GetAdjacentCell(Vector2Int direction)
    {
        return AdjacentCells[direction.x + 1, direction.y + 1];
    }

    public bool SetUnit(UnitScript unit) //Returns true if successful
    {
        if (attachedUnit)
        {
            return false;
        }

        var transform1 = transform;
        unit.transform.position = transform1.position;
        unit.gameObject.transform.parent = transform1;
        unit.currentCell = this;
        attachedUnit = unit;
        LightDown();
        return true;
    }

    public bool SetStructure(LandStructure structure) //Returns true if successful
    {
        if (attachedStructure != null)
        {
            Debug.Log("Structure already exists");
            return false;
        }

        var landTypeIsCorrect = false;
        foreach (var land in landType)
        {
            if (structure.canBePlacedOn.Contains(land))
            {
                landTypeIsCorrect = true;
                break;
            }
        }
        
        if (!landTypeIsCorrect)
        {
            return false;
        }


        var newStructure = new GameObject();
        var spriteRenderer = newStructure.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 1;
        var structureScript = newStructure.AddComponent<StructureScript>();
        structureScript.landStructure = structure;
        structureScript.parentGridCell = this;
        var transform1 = transform;
        newStructure.transform.position = transform1.position;
        if (structure.rotatable)
        {
            newStructure.transform.Rotate(new Vector3(0, 0, GridEditor.Instance.rotationTimes * 90));
        }
        newStructure.transform.SetParent(transform1);
        
        attachedStructure = structureScript;
        return true;
    }

    public void RemoveStructure()
    {
        if (attachedStructure)
        {
            Destroy(attachedStructure.gameObject);
            attachedStructure = null;
            RefreshLand();
        }
        
    }

    public void RefreshLand()
    {
        landType.Clear();
        landType.Add(landScapeCellSprites.landType);
        Walkable = true;
        movementCost = landScapeCellSprites.movementCost;
        
    }
    
    public void LightUp()
    {
        if (GridEditor.Instance)
        {
            if (GridEditor.Instance.landStructureToPlace)
            {
                var canBePlaced = false;

                foreach (var land in landType)
                {
                    if (GridEditor.Instance.landStructureToPlace.canBePlacedOn.Contains(land))
                    {
                        canBePlaced = true;
                        break;
                    }
                }

                if (canBePlaced && !attachedStructure)
                {
                    spriteRenderer.color = Color.green;
                }
                else
                {
                    spriteRenderer.color = Color.red;
                }

                return;
            }
            else
            {
                spriteRenderer.color = Color.yellow;
            }
        }

        spriteRenderer.color = Color.yellow;
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }

    public void LightDown()
    {
        spriteRenderer.color = Color.white;
    }
}