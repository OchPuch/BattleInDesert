using System;
using ScriptableObjects;
using UnityEngine;

namespace UI
{
    public class BuildChooser : MonoBehaviour
    {
        [Header("River")]
        public GameObject riverPrefab;
        [Header("Road")]
        public GameObject roadPrefab;
        [Header("Nature")]
        public GameObject naturePrefab;
        [Header("Buildings")]
        public GameObject buildingsPrefab;
        [Header("AirPort")]
        public GameObject airPortPrefab;
        [Header("LandScape")]
        public GameObject landScapePrefab;
    
        public void Awake()
        {
            riverPrefab.SetActive(false);
            roadPrefab.SetActive(false);
            naturePrefab.SetActive(false);
            buildingsPrefab.SetActive(false);
            airPortPrefab.SetActive(false);
            landScapePrefab.SetActive(false);
        }
    
        public void DisableEverything()
        {
            riverPrefab.SetActive(false);
            roadPrefab.SetActive(false);
            naturePrefab.SetActive(false);
            buildingsPrefab.SetActive(false);
            airPortPrefab.SetActive(false);
            landScapePrefab.SetActive(false);
        }

        public void Enable(String structureType)
        {
            if (structureType == "LandScape")
            {
                DisableEverything();
                landScapePrefab.SetActive(true);
                return;
            }
            
            var structure = (LandStructure.StructureType) Enum.Parse(typeof(LandStructure.StructureType), structureType);
            DisableEverything();
            switch (structure)
            {
                case LandStructure.StructureType.River:
                    riverPrefab.SetActive(true);
                    break;
                case LandStructure.StructureType.Road:
                    roadPrefab.SetActive(true);
                    break;
                case LandStructure.StructureType.Nature:
                    naturePrefab.SetActive(true);
                    break;
                case LandStructure.StructureType.Building:
                    buildingsPrefab.SetActive(true);
                    break;
                case LandStructure.StructureType.AirPort:
                    airPortPrefab.SetActive(true);
                    break;
                default:
                    DisableEverything();
                    break;
            }
        }
    }
}
