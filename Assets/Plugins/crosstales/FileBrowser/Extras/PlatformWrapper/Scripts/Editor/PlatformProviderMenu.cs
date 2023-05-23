#if UNITY_EDITOR
using UnityEditor;
using Crosstales.FB.EditorUtil;

namespace Crosstales.FB.EditorIntegration
{
   /// <summary>Editor component for the "Tools"-menu.</summary>
   public static class PlatformProviderMenu
   {
      [MenuItem("Tools/" + Crosstales.FB.Util.Constants.ASSET_NAME + "/Prefabs/PlatformWrapper", false, EditorHelper.MENU_ID + 40)]
      private static void AddPlatformProvider()
      {
         EditorHelper.InstantiatePrefab("PlatformWrapper", $"{EditorConfig.ASSET_PATH}Extras/PlatformWrapper/Resources/Prefabs/");
      }

      [MenuItem("Tools/" + Crosstales.FB.Util.Constants.ASSET_NAME + "/Prefabs/PlatformWrapper", true)]
      private static bool AddPlatformProviderValidator()
      {
         return !EditorExtension.PlatformWrapperEditor.isPrefabInScene;
      }
   }
}
#endif
// © 2021-2022 crosstales LLC (https://www.crosstales.com)