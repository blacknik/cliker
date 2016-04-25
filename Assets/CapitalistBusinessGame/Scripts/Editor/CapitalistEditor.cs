using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class CapitalistEditor : EditorWindow
{
    //This is used for openning persistent data folder,
    //because by default it's hard to find it
    [MenuItem("Window/Capitalist/Open Persistent Data folder")]
    public static void ShowExplorer()
    {
        string itemPath = Application.persistentDataPath;
        itemPath = itemPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
        System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
    }

//------------------------EDITOR WILL BE ADDED IN FURTHER VERSION------------------------------//
}