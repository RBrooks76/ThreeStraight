using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace hyhy.Common
{
    public static class ExpandedDropdownMenuOptions
    {
        [MenuItem("GameObject/UI/hyhy/ExpandedDropdown", false, 2035)]
        static public void AddExpandedDropdown(MenuCommand menuCommand)
        {
            GameObject go;
            using (new UnityEditor.UI.ExpandedMenuOptions.FactorySwapToEditor())
                go = ExpandedControls.CreateExpandedDropdown(ExpandedMenuOptions.GetStandardResources());
            ExpandedMenuOptions.PlaceUIElementRoot(go, menuCommand);
        }

    }
}