using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Unit", menuName = "Unit")]
    public class Unit : ScriptableObject
    {
        [Header("General")] public new string name;
        public string description;
        public Sprite spriteTeam1;
        [HideInInspector] public Sprite spriteTeam2;
        [HideInInspector] public Sprite spriteDestroyed;
        public int health;


        [Header("Movement")] public bool canDriveWater;
        public bool canDriveLand;

        [FormerlySerializedAs("canDriveRiver")]
        public bool canDriveShallowWater;

        public float actionPoints;

        [Header("Attack")] public int sightRange;
        public int attackRange;

        [FormerlySerializedAs("attackRangeBouns")]
        public int attackRangeBonus;

        public int damage;


        public string path;

        #if UNITY_EDITOR
        public void OnValidate()
        {
            path = UnityEditor.AssetDatabase.GetAssetPath(this);
        }
        #endif
    }
}