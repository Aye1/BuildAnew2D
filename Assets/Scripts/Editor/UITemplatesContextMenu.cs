using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UITemplatesContextMenu : MonoBehaviour
{
    [MenuItem("GameObject/BuildAnew/Button", false, 10)]
    static void CreateButtonTemplate(MenuCommand menuCommand)
    {
        GameObject go = (GameObject) Resources.Load("TemplatesGameObjects/ButtonTemplate");
        GameObject newGo = Instantiate(go);
        newGo.name = "Button (BA)";
        GameObjectUtility.SetParentAndAlign(newGo, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(newGo, "Create " + newGo.name);
        Selection.activeObject = newGo;
    }
}
