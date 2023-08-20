using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Managers;
using ScriptableObjects;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneTemplate;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class GridCellSerialization
{
    public int gridPositionX, gridPositionY;
    public String landScapeCellPath;
    public String landStructurePath;
    public float structureRotationTimes;
    public int teamId;

    public GridCellSerialization(GridCell gridCell)
    {
        gridPositionX = gridCell.gridPosition.x;
        gridPositionY = gridCell.gridPosition.y;
        structureRotationTimes =
            gridCell.attachedStructure != null ? gridCell.attachedStructure.gameObject.transform.eulerAngles.z : 0;
        landScapeCellPath = gridCell.landScapeCell.path;
        landStructurePath = gridCell.attachedStructure
            ? gridCell.attachedStructure.landStructure.path
            : "";
        teamId = gridCell.teamId;
    }

    public GridCell CreateGridCell()
    {
        if (landStructurePath != string.Empty)
        {
            GridManager.GridGenerated += CreateStructure;
        }
        
        if (GridManager.Instance.gridCells.Count == GridManager.GridBoundX * GridManager.GridBoundY)
        {
            Debug.Log("Grid is full");
            return null;
        }

        var cell = Object.Instantiate(GameData.Instance.gridCellPrefab, GridManager.Instance.transform.position,
            Quaternion.identity);
        cell.gridPosition = new Vector2Int(gridPositionX, gridPositionY);
        cell.landScapeCell = Resources.Load<LandScapeCell>(landScapeCellPath);
        cell.transform.SetParent( GridManager.Instance.transform);
        cell.teamId = teamId;
        

        return cell;
    }

    public void CreateStructure()
    {
        var gridLandscape = Resources.Load<LandScapeCell>(landScapeCellPath);

        if (landStructurePath == string.Empty)
        {
            return;
        }

        var structure = (Resources.Load<LandStructure>(landStructurePath));


        if (!structure.canBePlacedOn.Contains(gridLandscape.landType))
        {
            return;
        }

        if (!GridManager.Instance)
        {
            Debug.Log("GridManager not found");
            return;
        }
        
        

        var newStructure = Object.Instantiate(GameData.Instance.structurePrefab,
            GridManager.Instance.transform.position, Quaternion.identity);

        newStructure.landStructure = structure;
        newStructure.parentGridCell = GridManager.Instance.GetGridCell(new Vector2Int(gridPositionX, gridPositionY));
        
        
        var transform1 = newStructure.parentGridCell.transform;
        newStructure.transform.position = transform1.position;
        if (structure.rotatable)
        {
            newStructure.transform.Rotate(new Vector3(0, 0, structureRotationTimes));
        }

        newStructure.transform.SetParent(transform1);
    }
}