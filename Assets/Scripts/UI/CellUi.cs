using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    public class CellUi : MonoBehaviour
    {
        [HideInInspector]
        public RawImage Image;
        [HideInInspector]
        public LandStructure landStructure;
        [HideInInspector]
        public LandScapeCell landScapeCell;
        
        private void Awake()
        {
            Image = GetComponent<RawImage>();
        }

        public void OnClick()
        {
            if (landStructure) GridEditor.Instance.ChooseLandStructure(landStructure);
            else if (landScapeCell) GridEditor.Instance.ChooseLandScape(landScapeCell);
        }
    }
}
