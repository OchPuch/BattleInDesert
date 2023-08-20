using System;
using UI;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData", order = 1)]
    public class GameData : ScriptableObject
    {
        public static GameData Instance;
        public GridCell gridCellPrefab;
        public StructureScript structurePrefab;
        [Header("PopUps")]
        public CellPopUp cellPopUpPrefab;
        public ErrorPopUp errorPopUpPrefab;
        
        public static void Init()
        {
            Debug.Log("GameData Init");
            Instance = Resources.LoadAll<GameData>("GameData")[0];
            if (!Instance)
            {
                Debug.Log("GameData not found");
            }
        }
    }
}