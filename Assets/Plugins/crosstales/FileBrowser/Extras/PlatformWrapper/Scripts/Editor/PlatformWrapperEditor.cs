#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Crosstales.FB.EditorExtension
{
   /// <summary>Custom editor for the 'PlatformWrapper'-class.</summary>
   [CustomEditor(typeof(Crosstales.FB.Tool.PlatformWrapper))]
   [CanEditMultipleObjects]
   public class PlatformWrapperEditor : Editor
   {
      #region Variables

      private Crosstales.FB.Tool.PlatformWrapper script;

      #endregion


      #region Properties

      public static bool isPrefabInScene => GameObject.Find("PlatformWrapper") != null;

      #endregion


      #region Editor methods

      private void OnEnable()
      {
         script = (Crosstales.FB.Tool.PlatformWrapper)target;
      }

      public override void OnInspectorGUI()
      {
         DrawDefaultInspector();

         if (script.isActiveAndEnabled)
         {
            //do something
         }
         else
         {
            Crosstales.FB.EditorUtil.EditorHelper.SeparatorUI();
            EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
         }
      }

      #endregion
   }
}
#endif
// © 2021-2022 crosstales LLC (https://www.crosstales.com)