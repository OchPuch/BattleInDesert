using System;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Landscape Cell", menuName = "Cell")]
    [Serializable]
    public class LandScapeCell : ScriptableObject
    {
        [Header("General")]
        public Sprite sprite;
        [Header("Movement")]
        public float movementCost;
        public LandType landType;
        
        public enum LandType
        {
            Land,
            Water,
            ShallowWater
        }
        
        public string path;

        #if UNITY_EDITOR
        public void OnValidate()
        {
            path = UnityEditor.AssetDatabase.GetAssetPath(this);
            path = path.Replace(".asset", "");
            path = path.Replace("Assets/Resources/", "");
        }
        #endif
    }
}