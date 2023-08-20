using System;
using System.Collections.Generic;
using ScriptableObjects;
using UI;
using UnityEngine;

namespace Managers
{
    public class ErrorManager : MonoBehaviour
    {
        private static ErrorManager _instance;
        private GameObject _canvas;
        public List<ErrorPopUp> errorPopUps = new List<ErrorPopUp>();

        private void Awake()
        {
            if (!_instance)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            _canvas = GameObject.Find("Canvas");
        }
        
        public static void ShowError(string title, string text)
        {
            var popUp = Instantiate(GameData.Instance.errorPopUpPrefab, _instance.transform);
            popUp.transform.SetParent(_instance._canvas.transform);
            popUp.transform.localPosition = Vector3.zero;
            popUp.SetTitle(title);
            popUp.SetText(text);
            popUp.CloseButton.onClick.AddListener( () => _instance.ClosePopUp(popUp));
            _instance.errorPopUps.Add(popUp);
        }
        
        public void ClosePopUp(ErrorPopUp popUp)
        {
            errorPopUps.Remove(popUp);
            Destroy(popUp.gameObject);
        }
    }
}