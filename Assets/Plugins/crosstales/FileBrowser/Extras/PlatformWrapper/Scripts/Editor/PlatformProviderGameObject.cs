#if UNITY_EDITOR
using UnityEditor;
using Crosstales.FB.EditorUtil;

namespace Crosstales.FB.EditorIntegration
{
   /// <summary>Editor component for the "Hierarchy"-menu.</summary>
   public static class PlatformProviderGameObject
   {
      [MenuItem("GameObject/" + Crosstales.FB.Util.Constants.ASSET_NAME + "/PlatformWrapper", false, EditorHelper.GO_ID + 2)]
      private static void AddPlatformWrapper()
      {
         EditorHelper.InstantiatePrefab("PlatformWrapper", $"{EditorConfig.ASSET_PATH}Extras/PlatformWrapper/Resources/Prefabs/");
      }

      [MenuItem("GameObject/" + Crosstales.FB.Util.Constants.ASSET_NAME + "/PlatformWrapper", true)]
      private static bool AddPlatformWrapperValidator()
      {
         return !EditorExtension.PlatformWrapperEditor.isPrefabInScene;
      }
   }
}
#endif
// © 2021-2022 crosstales LLC (https://www.crosstales.com)