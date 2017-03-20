using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SF;

public class SFScriptHeaderGenerator : UnityEditor.AssetModificationProcessor
{
    static private string header =
        "/**\n" +
        " * Created on ##DateTime## by ##UserName##\n" +
        " * All rights reserved.\n" +
        " */\n\n";

    public static void OnWillCreateAsset(string path)
    {
        path = path.Replace(".meta", "");
        if (path.EndsWith(".cs"))
        {
            string fullText = header;
            fullText = fullText.Replace("##DateTime##", System.DateTime.Now.ToString("yyyy/MM/dd"));
            fullText = fullText.Replace("##UserName##", System.Environment.UserName);
            fullText += File.ReadAllText(path);
            File.WriteAllText(path, fullText);
        }
    }
}

public class SFCustomMenu
{
    [MenuItem("Assets/SF/Export UI")]
    private static void exportUI()
    {
        SFEditorUtils.generateUICode(Selection.activeGameObject, false);
    }

    [MenuItem("Assets/SF/Export UI", true, 1)]
    private static bool exportUIValidation()
    {
        return SFEditorUtils.checkUIValidation(Selection.activeGameObject);
    }

    [MenuItem("Assets/SF/Export UI With Presenter")]
    private static void exportUIWithPresenter()
    {
        SFEditorUtils.generateUICode(Selection.activeGameObject, true);
    }

    [MenuItem("Assets/SF/Export UI With Presenter", true, 2)]
    private static bool exportUIWithPresenterValidation()
    {
        return SFEditorUtils.checkUIValidation(Selection.activeGameObject);
    }
}
