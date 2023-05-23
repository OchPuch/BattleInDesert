using System.Collections.Generic;
using Controllers;
using UnityEngine;

namespace Managers
{
    public class PopUpManager : MonoBehaviour
    {
        public static PopUpManager Instance;
        [SerializeField] private GameObject cellCostPopUp;
        private List<GameObject> _cellCostPopUps = new List<GameObject>();

        [SerializeField] private GameObject cellCostParent;
        private Camera _camera;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            _camera = Camera.main;
            PlayerInteraction.OnUnitDeselected += DestroyCellCostPopUps;
        }

        public void ShowCellCostPopUp()
        {
            var path = PlayerInteraction.SelectedPath;
            foreach (var cell in path)
            {
                CreateCellPopUp(cell, cell.movementCost);
            }
        }

        public void CreateCellPopUp(GridCell cell, float coast)
        {
            var popup = Instantiate(cellCostPopUp, _camera.WorldToScreenPoint(cell.transform.position),
                Quaternion.identity);
            popup.transform.SetParent(cellCostParent.transform);
            popup.GetComponent<CellCostPopUp>().cost = coast;
            _cellCostPopUps.Add(popup);
        }

        private void DestroyCellCostPopUps()
        {
            foreach (var popUp in _cellCostPopUps)
            {
                Destroy(popUp);
            }
        }
    }
}