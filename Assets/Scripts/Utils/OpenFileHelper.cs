using Crosstales.FB;
using UnityEditor;


namespace Utils
{
    public static class OpenFileHelper
    {
        public static string GetPathToLoadJsonFile()
        {
            string[] extensions = new string[] { "json" }; 
            return FileBrowser.Instance.OpenSingleFile("Open JSON Map", "", "", extensions);
        }
        
        public static string GetPathToSaveJsonFile()
        {
            string[] extensions = new string[] { "json" };
            return FileBrowser.Instance.SaveFile("Save JSON Map", "", "NewMap", extensions);
        }
        
        public static string GetPathToLoadImageFile()
        {
            string[] extensions = new string[] { "png", "jpg", "jpeg" };
            return FileBrowser.Instance.OpenSingleFile("Load landscape from image", "", "", extensions);
        }
        
        
    }
}