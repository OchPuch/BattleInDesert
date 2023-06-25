using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Structure", menuName = "Structure")]
    [Serializable]
    public class LandStructure : ScriptableObject
    {
        [Header("General")]
        public Sprite sprite;
        public bool rotatable; //Editor
        public LandScapeCell.LandType[] canBePlacedOn; //Editor
        public bool canBeBuiltOn; //Editor
        [Header("Destruction")]
        public LandStructure afterDestruction;
        public Unit[] canBeDestroyedBy; //If 0, can be destroyed by any unit
        public bool destructible;
        public int health;
        [Header("Movement")]
        public bool drivable;
        public Unit[] canBeDrivenBy; //If 0, can be driven by any unit
        public bool settedLandType;
        public LandScapeCell.LandType setLandType;
        public bool addedLandType;
        public LandScapeCell.LandType addLandType;
        public float movementCostSet;
        public float movementCostAdd;
        public float movementCostMultiply;
        [Header("Another interaction")]    
        public bool canShootThrough;
        public bool canHideInside;
        public Unit canHideThere;


        public enum StructureType
        {
            River,
            Road,
            AirPort,
            Nature,
            Building
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