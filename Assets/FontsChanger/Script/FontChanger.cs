using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class FontChanger : EditorWindow
{
    private static FontChanger window;

    private Font chageFont;
    private int changeFontSize;

    [MenuItem("Tools/Font Switcher")]
    private static void Init()
    {
        window = GetWindow<FontChanger>();
    }

    private void OnGUI()
    {
        chageFont = EditorGUILayout.ObjectField("Font", chageFont, typeof(Font), false) as Font;
        changeFontSize = EditorGUILayout.IntField("FontSize", changeFontSize);
        
        GUILayout.Space(10f);
        if (GUILayout.Button("Apply"))
            ChangeFont(chageFont,changeFontSize);
        else if (GUILayout.Button("Reset"))
            ChangeFont(null,changeFontSize);
    }

    private void ChangeFont(Font font, int size)
    {
        BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty;
        PropertyInfo[] ediorStyleeInfos = typeof(EditorStyles).GetProperties(flags);
        PropertyInfo[] guiStyleInfos = GUI.skin.GetType().GetProperties();

        for (int i = 0; i < ediorStyleeInfos.Length; i++)
        {
            if (PropertyInfoExists(ediorStyleeInfos[i]))
            {
                GUIStyle style = ediorStyleeInfos[i].GetValue(null, null) as GUIStyle;
                style.font = font;
                style.fontSize = size;
            }
        }

        for (int i = 0; i < GUI.skin.customStyles.Length; i++)
        {
                GUI.skin.customStyles[i].font = font;
                GUI.skin.customStyles[i].fontSize = size;
        }
        
        EditorWindow[] windows = Resources.FindObjectsOfTypeAll<EditorWindow>();
        for (int i = 0; i < windows.Length; i++)
        {
            windows[i].Repaint();
        }
        
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }

    private bool PropertyInfoExists(PropertyInfo info)
    {
        if (string.IsNullOrEmpty(info.Name))
            return false;
        else if (info.PropertyType != typeof(GUIStyle))
            return false;
        else
            return true;
    }
}