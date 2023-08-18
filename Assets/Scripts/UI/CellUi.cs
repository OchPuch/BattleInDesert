using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace UI
{
    public class CellUi : MonoBehaviour
    {
        [SerializeField]
        public RawImage image;
        [HideInInspector]
        public LandStructure landStructure;
        [HideInInspector]
        public LandScapeCell landScapeCell;
        [SerializeField] 
        private TextMeshProUGUI text;
        
        private void Awake()
        {
            if (!image) image = GetComponent<RawImage>();
        }

        public void OnClick()
        {
            if (landStructure) GridEditor.Instance.ChooseLandStructure(landStructure);
            else if (landScapeCell) GridEditor.Instance.ChooseLandScape(landScapeCell);
        }
        
    }
}
