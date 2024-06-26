using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Mirror;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

public class StructureScript : MonoBehaviour, IDamagable
{
    public LandStructure landStructure;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public UnitScript hidedUnit;
    public GridCell parentGridCell;
    public float healthPointsLeft;
    
    
    public void Start()
    {
        
        if (!parentGridCell)
        {
            Destroy(gameObject);
        }
        if (!spriteRenderer)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        healthPointsLeft = landStructure.health;
        spriteRenderer.sprite = landStructure.sprite;

        if (landStructure.addedLandType)
        {
            parentGridCell.landType.Add(landStructure.addLandType);
        }

        if (landStructure.settedLandType)
        {
            parentGridCell.landType.Clear();
            parentGridCell.landType.Add(landStructure.setLandType);
        }

        if (!landStructure.drivable)
        {
            parentGridCell.walkable = false;
        }
        else if (landStructure.movementCostSet != 0 )
        {
            parentGridCell.movementCost = landStructure.movementCostSet;
        }
        else if (landStructure.movementCostAdd != 0)
        {
            parentGridCell.movementCost += landStructure.movementCostAdd;
        }
        else if (landStructure.movementCostMultiply != 0)
        {
            parentGridCell.movementCost *= landStructure.movementCostMultiply;
        }
        
       
        parentGridCell.attachedStructure = this;
    }

    public void TakeDamage(int damage, UnitScript damager)
    {
        if (landStructure.destructible)
        {
            if (landStructure.canBeDestroyedBy.Contains(damager.unit))
            {
                healthPointsLeft -= damage;
                if (healthPointsLeft <= 0)
                {
                    Destroy(gameObject);
                }
            }
            
            healthPointsLeft -= damage;
            if (healthPointsLeft <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void HideInside(UnitScript unitToHide)
    {
        
    }
}
