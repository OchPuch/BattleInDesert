using UnityEngine;
using System;

namespace Crosstales.FB.Tool
{
   /// <summary>Allows to configure wrappers per platform.</summary>
   //[ExecuteInEditMode]
   [HelpURL("https://www.crosstales.com/media/data/assets/FileBrowser/api/class_crosstales_1_1_f_b_1_1_tool_1_1_platform_wrapper.html")]
   public class PlatformWrapper : MonoBehaviour
   {
      [Header("Configuration Settings"), Tooltip("Platform specific wrapper for the app (empty wrapper = default of the OS).")]
      public PlatformWrapperTuple[] Configuration;

      [Header("Default"), Tooltip("Default wrapper of the app (empty = default of the OS).")] public Crosstales.FB.Wrapper.BaseCustomFileBrowser DefaultWrapper;

      [Header("Parenting"), Tooltip("Set the provider as child of the FileBrowser parent object (default: true).")]
      public bool SetAsChild = true;

      [Header("Editor"), Tooltip("Use the default wrapper inside the Editor (default: false).")] public bool UseDefault;

      private void Start()
      {
         bool found = false;

         if (!Crosstales.FB.Util.Helper.isEditor || !UseDefault)
         {
            Crosstales.Common.Model.Enum.Platform currentPlatform = Crosstales.FB.Util.Helper.CurrentPlatform;

            foreach (PlatformWrapperTuple config in Configuration)
            {
               if (config.Platform == currentPlatform)
               {
                  if (config.CustomWrapper == null)
                  {
                     FileBrowser.Instance.CustomMode = false;
                  }
                  else
                  {
                     FileBrowser.Instance.CustomWrapper = config.CustomWrapper;
                     FileBrowser.Instance.CustomMode = true;

                     if (SetAsChild)
                     {
                        for (int ii = FileBrowser.Instance.transform.childCount - 1; ii >= 0; ii--)
                        {
                           Transform child = FileBrowser.Instance.transform.GetChild(ii);
                           //child.SetParent(null);

                           if (child != config.CustomWrapper.transform)
                              Destroy(child.gameObject);
                        }

                        config.CustomWrapper.transform.SetParent(FileBrowser.Instance.transform);
                     }
                  }

                  found = true;
                  break;
               }
            }
         }

         if (!found)
         {
            if (DefaultWrapper == null)
            {
               FileBrowser.Instance.CustomMode = false;
            }
            else
            {
               FileBrowser.Instance.CustomWrapper = DefaultWrapper;
               FileBrowser.Instance.CustomMode = true;

               if (SetAsChild)
               {
                  for (int ii = FileBrowser.Instance.transform.childCount - 1; ii >= 0; ii--)
                  {
                     Transform child = FileBrowser.Instance.transform.GetChild(ii);
                     //child.SetParent(null);
                     Destroy(child.gameObject);
                  }

                  DefaultWrapper.transform.SetParent(FileBrowser.Instance.transform);
               }
            }
         }
      }
   }

   [Serializable]
   public class PlatformWrapperTuple
   {
      public Crosstales.Common.Model.Enum.Platform Platform;
      public Crosstales.FB.Wrapper.BaseCustomFileBrowser CustomWrapper;
   }
}
// © 2021-2022 crosstales LLC (https://www.crosstales.com)