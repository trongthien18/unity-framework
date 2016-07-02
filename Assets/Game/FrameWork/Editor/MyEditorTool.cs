using UnityEngine;
using UnityEditor;
using System.Collections;

public class MyEditorTool : MonoBehaviour
{
    [MenuItem("Tools/DeleteMyPlayerPrefs")]
    static void DeleteMyPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
