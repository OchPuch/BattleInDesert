using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Managers;
using ScriptableObjects;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[Serializable]
public class GridCellSerialization
{
    public int gridPositionX, gridPositionY;
    public String landScapeCellPath;
    public String landStructurePath;
    public float structureRotationTimes;
    
    public GridCellSerialization(GridCell gridCell)
    {
        gridPositionX = gridCell.gridPosition.x;
        gridPositionY = gridCell.gridPosition.y;
        structureRotationTimes =
            gridCell.attachedStructure != null ? gridCell.attachedStructure.gameObject.transform.eulerAngles.z  : 0;
        landScapeCellPath = gridCell.landScapeCellSprites.path;
        landStructurePath = gridCell.attachedStructure
            ? gridCell.attachedStructure.landStructure.path
            : "";
    }

    public GridCell CreateGridCell()
    {
        
        GridManager.GridGenerated += CreateStructure;
        if (GridManager.Instance.gridCells.Count == GridManager.GridBoundX * GridManager.GridBoundY)
        {
            Debug.Log("Grid is full");
            return null;
        }

        var cell = new GameObject("GridCell");
        cell.AddComponent<SpriteRenderer>();
        var gridCell = cell.AddComponent<GridCell>();
        gridCell.gridPosition = new Vector2Int(gridPositionX, gridPositionY);
        gridCell.landScapeCellSprites = Resources.Load<LandScapeCellSprites>(landScapeCellPath);
        gridCell.transform.parent = GridManager.Instance.transform;
        return gridCell;
    }

    public void CreateStructure()
    {
        var gridLandscape  = Resources.Load<LandScapeCellSprites>(landScapeCellPath);
        
        if (landStructurePath == "")
        {
            return;
        }

        var structure = (Resources.Load<LandStructure>(landStructurePath));
        

        if (!structure.canBePlacedOn.Contains(gridLandscape.landType))
        {
            return;
        }


        var newStructure = new GameObject("Structure");
        var spriteRenderer = newStructure.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 1;
        var structureScript = newStructure.AddComponent<StructureScript>();
        structureScript.landStructure = structure;
        structureScript.parentGridCell = GridManager.Instance.GetGridCell(new Vector2Int (gridPositionX, gridPositionY));
        var transform1 = structureScript.parentGridCell.transform;
        newStructure.transform.position = transform1.position;
        if (structure.rotatable)
        {
            newStructure.transform.Rotate(new Vector3(0, 0, structureRotationTimes));
        }

        newStructure.transform.SetParent(transform1);

        structureScript.parentGridCell.attachedStructure = structureScript;
    }
}