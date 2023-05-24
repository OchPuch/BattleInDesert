using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Managers;
using Mirror;
using Multiplayer;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class UnitScript : MonoBehaviour, IDamagable
{
    [SerializeField] public Unit unit;
    private float _actionPointsLeft;
    
    private SpriteRenderer _spriteRenderer;
    public GridCell currentCell;
    public float healthPointsLeft;

    public GridManager GridManager;
    
    void Start()
    {
        _actionPointsLeft = unit.actionPoints;
        healthPointsLeft = unit.health;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = unit.spriteTeam1;
        GridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        var position = transform.position;
        GridManager.GetGridCell(new Vector2Int(Mathf.RoundToInt( position.x/GridManager.CellSize.x), Mathf.RoundToInt(position.y/GridManager.CellSize.y))).SetUnit(this);
    }

    public void MoveToCell(GridCell targetCell)
    {
        var path = PathManager.Instance.PathFinder(currentCell, targetCell, this);
        if (path == null)
        {
            Debug.Log("No path found");
            return;
        }

        StartCoroutine(MoveToCellCoroutine(path));
    }
    
    public void AttackUnit(UnitScript targetUnit)
    {
        if ((_actionPointsLeft) > 0)
        {
            targetUnit.TakeDamage(unit.damage, this);
        }
    }

    

    public void TakeDamage(int damage, UnitScript damager)
    {
        healthPointsLeft -= damage;
        Debug.Log(healthPointsLeft);
        if (healthPointsLeft <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnGameStateChanged(GameState obj)
    {
        switch (obj)
        {
            case GameState.SettingUp:
                break;
            case GameState.Player1Turn:
                _actionPointsLeft = unit.actionPoints;
                break;
            case GameState.Player1TurnEnd:
                break;
            case GameState.Player2Turn:
                break;
            case GameState.Player2TurnEnd:
                break;
            case GameState.GameOver:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
        }
    }
    
    //Coroutine for moving the unit through each cell in the path to the target cell
    private IEnumerator MoveToCellCoroutine(List<GridCell> path)
    {
        foreach (var cell in path)
        {
            currentCell.attachedUnit = null;
            if (cell.SetUnit(this) )
            {
                _actionPointsLeft -= currentCell.movementCost;
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                currentCell.attachedUnit = this;
                break;
            }
        }
    }
}