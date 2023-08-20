using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controllers
{
    public class PlayerInteraction : MonoBehaviour
    {
        public static PlayerInteraction Instance;

        //Cells that the player can interact with
        public static GridCell SelectedCell;
        public static GridCell TargetCell;

        //Units that are selected
        public static UnitScript SelectedUnit;
        public static UnitScript TargetUnit;

        public static List<GridCell> SelectedPath;
        public static HashSet<GridCell> SelectedArea = new HashSet<GridCell>();

        //Visuals
        [FormerlySerializedAs("SelectedObject")]
        public GameObject selectedObject;

        public GameObject targetObject;

        public static event Action OnUnitSelected;
        public static event Action OnUnitDeselected;
        public static event Action OnTargetUnitSelected;
        public static event Action OnTargetCellSelected;
        public static event Action OnCellSelected;

        public static InteractionState CurrentState = InteractionState.Chilling;


        public enum InteractionState
        {
            Chilling,
            CellSelected,
            UnitSelected,
            TargetCellSelected,
            TargetUnitSelected,
            AreaSelected
        }


        public void Awake()
        {
            Instance = this;
            selectedObject.SetActive(false);
            targetObject.SetActive(false);
        }

        private void Update()
        {
            //If the player clicks the left mouse button and the mouse is over a cell but not over a UI element (like a button) then select the cell 
            if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                var cell = GridManager.Instance.MouseOverCell();
                if (!cell)
                {
                    CurrentState = InteractionState.Chilling;
                    PathManager.Instance.ClearPath();
                    RefreshSelected();
                    return;
                }

                //Что происходит когда ты жмёшь по клеточке в зависимости от состояния
                switch (CurrentState)
                {
                    case InteractionState.Chilling:
                        CurrentState = InteractionState.CellSelected;
                        SelectedCell = cell;
                        SelectedArea.Add(SelectedCell);
                        selectedObject.SetActive(true);
                        selectedObject.transform.position = cell.transform.position;
                        if (cell.attachedUnit)
                        {
                            CurrentState = InteractionState.UnitSelected;
                            Debug.Log("Unit Selected");
                            SelectedUnit = cell.attachedUnit;
                            OnUnitSelected?.Invoke();
                        }
                        else
                        {
                            OnCellSelected?.Invoke();
                        }

                        break;
                    case InteractionState.CellSelected:
                        SelectedCell = cell;
                        selectedObject.SetActive(true);
                        selectedObject.transform.position = cell.transform.position;
                        if (cell.attachedUnit)
                        {
                            CurrentState = InteractionState.UnitSelected;
                            Debug.Log("Unit Selected");
                            SelectedUnit = cell.attachedUnit;
                            OnUnitSelected?.Invoke();
                        }
                        else
                        {
                            OnCellSelected?.Invoke();
                        }

                        break;
                    case InteractionState.AreaSelected:
                        CurrentState = InteractionState.Chilling;
                        PathManager.Instance.ClearPath();
                        RefreshSelected();
                        break;
                    case InteractionState.UnitSelected:
                        TargetCell = cell;
                        targetObject.SetActive(true);
                        targetObject.transform.position = cell.transform.position;
                        if (cell.attachedUnit)
                        {
                            Debug.Log("Unit Targeted");
                            TargetUnit = cell.attachedUnit;
                            CurrentState = InteractionState.TargetUnitSelected;
                            OnTargetUnitSelected?.Invoke();
                        }
                        else
                        {
                            CurrentState = InteractionState.TargetCellSelected;
                            OnTargetCellSelected?.Invoke();
                        }

                        break;
                    case InteractionState.TargetCellSelected:
                        CurrentState = InteractionState.Chilling;
                        PathManager.Instance.ClearPath();
                        RefreshSelected();
                        break;
                    case InteractionState.TargetUnitSelected:
                        CurrentState = InteractionState.Chilling;
                        PathManager.Instance.ClearPath();
                        RefreshSelected();
                        break;
                    default:
                        CurrentState = InteractionState.Chilling;
                        PathManager.Instance.ClearPath();
                        RefreshSelected();
                        break;



                }
            }

            //if player hold left mouse button and the mouse is over a cell but not over a UI element (like a button) then select the cell
            if (Input.GetMouseButton(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                var cell = GridManager.Instance.MouseOverCell();
                
                //if cell exist and it is not the same as the selected cell
                if (cell && cell != SelectedCell)
                {
                    switch (CurrentState)
                    {
                        case InteractionState.CellSelected:
                            CurrentState = InteractionState.AreaSelected;
                            break;
                        case InteractionState.AreaSelected:
                            if (!SelectedArea.Contains(cell))
                            {
                                //find maximum position values of the selected area
                                var maxX = SelectedArea.Max(x => x.gridPosition.x);
                                var maxY = SelectedArea.Max(x => x.gridPosition.y);
                                var minX = SelectedArea.Min(x => x.gridPosition.x);
                                var minY = SelectedArea.Min(x => x.gridPosition.y);
                                
                                SelectedArea = GridManager.Instance.GetArea(SelectedCell, cell);
                                GridManager.Instance.LightUpArea(SelectedArea);
                            }
                            else
                            {
                                GridManager.Instance.LightDownArea();
                                SelectedArea.Clear();
                                SelectedArea = GridManager.Instance.GetArea(SelectedCell, cell);
                                GridManager.Instance.LightUpArea(SelectedArea);
                            }
                            break;
                    }
                }
            }
        }

        public void GoToTarget()
        {
            SelectedUnit.MoveToCell(TargetCell);
            RefreshSelected();
        }

        public void AttackTarget()
        {
            SelectedUnit.AttackUnit(TargetUnit);
            RefreshSelected();
        }

        private void RefreshSelected()
        {
            SelectedCell = null;
            SelectedUnit = null;
            selectedObject.SetActive(false);
            GridManager.Instance.LightDownArea();
            SelectedArea.Clear();
            TargetCell = null;
            TargetUnit = null;
            targetObject.SetActive(false);
            OnUnitDeselected?.Invoke();
            
        }
    }
}