using System;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData", order = 1)]
    public class GameData : ScriptableObject
    {
        public static GameData Instance;
        public GridCell gridCellPrefab;
        public StructureScript structurePrefab;

        public void OnEnable()
        {
            Instance = Resources.LoadAll<GameData>("GameData")[0];
            if (!Instance)
            {
                Debug.Log("GameData not found");
            }
        }

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