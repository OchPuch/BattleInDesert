using System.Collections.Generic;
using Controllers;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class PathManager : MonoBehaviour
    {
        //Show in inspector
        public static PathManager Instance;
       

        public void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            
            PlayerInteraction.OnTargetCellSelected += LightUpPath;
        }
        

        public List<GridCell> PathFinder(GridCell start, GridCell target, UnitScript walker)
        {
            var openList = new List<GridCell>();
            var closedList = new List<GridCell>();
            openList.Add(start);

            foreach (var cell in GridManager.Instance.gridCells)
            {
                cell.gCost = int.MaxValue;
                cell.parent = null;
            }

            start.gCost = 0;
            start.hCost = GetDistance(start, target);

            while (openList.Count > 0)
            {
                var current = openList[0];
                for (var i = 1; i < openList.Count; i++)
                {
                    if (openList[i].fCost < current.fCost ||
                        openList[i].fCost == current.fCost && openList[i].hCost < current.hCost)
                    {
                        current = openList[i];
                    }
                }

                openList.Remove(current);
                closedList.Add(current);

                if (current == target)
                {
                    return RetracePath(start, target);
                }

                foreach (var neighbour in current.AdjacentCells)
                {
                    if (!neighbour) continue;

                    if (closedList.Contains(neighbour) || !neighbour.IsWalkable(walker))
                    {
                        continue;
                    }

                    var newMovementCostToNeighbour =
                        (current.gCost + GetDistance(current, neighbour) * neighbour.movementCost);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openList.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, target);
                        neighbour.parent = current;

                        if (!openList.Contains(neighbour))
                        {
                            openList.Add(neighbour);
                        }
                    }
                }
            }

            return null;
        }

        private List<GridCell> RetracePath(GridCell start, GridCell end)
        {
            var path = new List<GridCell>();
            var current = end;

            while (current != start)
            {
                path.Add(current);
                current = current.parent;
            }

            path.Reverse();
            return path;
        }

        private float GetDistance(GridCell cellA, GridCell cellB)
        {
            var distanceX = Mathf.Abs(cellA.gridPosition.x - cellB.gridPosition.x);
            var distanceY = Mathf.Abs(cellA.gridPosition.y - cellB.gridPosition.y);

            if (distanceX > distanceY)
            {
                return 14 * distanceY + 10 * (distanceX - distanceY);
            }

            return 14 * distanceX + 10 * (distanceY - distanceX);
        }

        public void LightUpPath()
        {
            PlayerInteraction.SelectedPath = PathFinder(PlayerInteraction.SelectedCell, PlayerInteraction.TargetCell,
                PlayerInteraction.SelectedUnit);
            if (PlayerInteraction.SelectedPath == null)
            {
                return;
            }

            float cost = 0;

            foreach (var cell in PlayerInteraction.SelectedPath)
            {
                cost += cell.movementCost;
                PopUpManager.Instance.CreateCellPopUp(cell, cost);
                cell.LightUp();
            }
        }

        public void ClearPath()
        {
            if (PlayerInteraction.SelectedPath == null)
            {
                return;
            }

            foreach (var cell in PlayerInteraction.SelectedPath)
            {
                cell.LightDown();
            }

            PlayerInteraction.SelectedPath = null;
        }
    }
}