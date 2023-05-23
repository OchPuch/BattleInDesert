using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

public class AddCellsToUI : MonoBehaviour
{
    [SerializeField]
    private GameObject cellUiPrefab;
    
    public LandStructure[] landStructures;
    
    public LandStructure.StructureType structureTypeToLoad;
    
    
    
    private string GetPathToStructures()
    {
        switch (structureTypeToLoad)
        {
            case LandStructure.StructureType.River:
                return "Structures/River";
            case LandStructure.StructureType.Building:
                return "Structures/Buildings";
            case LandStructure.StructureType.AirPort:
                return "Structures/AirPort";
            case LandStructure.StructureType.Road:
                return "Structures/Asphalt_Road";
            case LandStructure.StructureType.Nature:
                return "Structures/Nature";
            default:
                return "Structures";
        }
    }
    
    
    private void Awake()
    {
        landStructures = Resources.LoadAll<LandStructure>(GetPathToStructures());
        //Instantiate UI elements for each structure
        foreach (var landStructure in landStructures)
        {
             var nigger =Instantiate(cellUiPrefab, transform.position, Quaternion.identity);
             var pidoras = nigger.GetComponent<CellUi>();
             pidoras.Image.texture = landStructure.sprite.texture;
             pidoras.landStructure = landStructure;
             nigger.transform.SetParent(transform);
        }
        
    }
}

