using Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class ButtonManager : MonoBehaviour
    {
        public Button attackButton;
        public Button moveButton;

        private void Awake()
        {
            attackButton.interactable = false;
            moveButton.interactable = false;


            PlayerInteraction.OnTargetCellSelected += EnableMoveButton;
            PlayerInteraction.OnTargetUnitSelected += EnableAttackButton;
            PlayerInteraction.OnUnitDeselected += DisableButtons;
        }

        private void DisableButtons()
        {
            attackButton.interactable = false;
            moveButton.interactable = false;
        }

        private void EnableAttackButton()
        {
            attackButton.interactable = true;
        }

        private void EnableMoveButton()
        {
            moveButton.interactable = true;
        }
    }
}