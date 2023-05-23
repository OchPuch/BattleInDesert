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
    private SpriteRenderer _spriteRenderer;
    public UnitScript hidedUnit;
    public GridCell parentGridCell;
    public float healthPointsLeft;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    
    public void Start()
    {
        if (!parentGridCell)
        {
            Destroy(gameObject);
        }
        if (!_spriteRenderer)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        healthPointsLeft = landStructure.health;
        _spriteRenderer.sprite = landStructure.sprite;

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
            parentGridCell.Walkable = false;
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
