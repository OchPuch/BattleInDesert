using UnityEngine;

namespace Managers
{
    public class MenuManager : MonoBehaviour
    {
        public GameObject networkManager;
        //Переход не сцену Editor
        public void GoToEditor()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Editor");
            Destroy(networkManager);
        }

        public void GoToMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    
    }
}
