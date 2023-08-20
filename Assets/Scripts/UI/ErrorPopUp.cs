using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ErrorPopUp : MonoBehaviour
    {
        public TextMeshProUGUI errorText;
        public TextMeshProUGUI errorTitle;
        public Button CloseButton;
        
        public void SetText(string s)
        {
            errorText.text = s;
        }
        
        public void SetTitle(string s)
        {
            errorTitle.text = s;
        }
    }
}