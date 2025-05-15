#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class Startup
{
    static Startup()    
    {
        EditorPrefs.SetInt("showCounts_sportcarFFF", EditorPrefs.GetInt("showCounts_sportcarFFF") + 1);

        if (EditorPrefs.GetInt("showCounts_sportcarFFF") == 1)       
        {
            Application.OpenURL("https://assetstore.unity.com/packages/slug/304757");
            // System.IO.File.Delete("Assets/SportCar/Racing_Game.cs");
        }
    }   
}
#endif
