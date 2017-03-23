/**
 * Created on 2017/03/20 by Inspoy
 * All rights reserved.
 */

using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SF
{
    public class SFEditorUtils
    {
        /// <summary>
        /// 检查给定的prefab是否是一个合法的UI View
        /// </summary>
        /// <param name="prefab">给定的GameObject</param>
        /// <returns>是否合法</returns>
        public static bool checkUIValidation(GameObject prefab)
        {
            bool check = false;
            if (prefab != null)
            {
                string prefabName = prefab.name;
                if (prefabName.Substring(0, 2) == "vw")
                {
                    check = true;
                }
            }
            return check;
        }

        /// <summary>
        /// 生成UI代码，并自动给Prefab挂载View脚本
        /// </summary>
        /// <param name="prefab">指定的UI View</param>
        /// <param name="exportPresenter"></param>
        public static void generateUICode(GameObject prefab, bool exportPresenter)
        {
            string viewName = prefab.name.Substring(2);
            Debug.Log(string.Format("Generating SF{0}View{1}...", viewName, exportPresenter ? " with presenter" : ""));

            string viewContent = "";
            string presenterContent = "";
            getViewContent(prefab, out viewContent, out presenterContent);

            string viewFilepath = string.Format("Assets/Scripts/UI/SF{0}View.cs", viewName);
            var viewFile = new FileInfo(viewFilepath);
            var sw = viewFile.CreateText();
            sw.Write(viewContent);
            sw.Close();
            // 添加文件头部的注释
            SFScriptHeaderGenerator.OnWillCreateAsset(viewFilepath);

            if (exportPresenter)
            {
                string presenterFilepath = string.Format("Assets/Scripts/UI/SF{0}Presenter.cs", viewName);
                var presenterFile = new FileInfo(presenterFilepath);
                var sw2 = presenterFile.CreateText();
                sw2.Write(presenterContent);
                sw2.Close();
                // 添加文件头部的注释
                SFScriptHeaderGenerator.OnWillCreateAsset(presenterFilepath);
            }

            // 给Prefab挂载View脚本
            string componentName = "SF" + viewName + "View";
            AssetDatabase.ImportAsset(viewFilepath);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            var component = prefab.GetComponent(componentName);
            if (component == null)
            { 
                prefab.AddComponent(getTypeByName(componentName));
            }
            Debug.Log("Generated!");
        }

        private static Type getTypeByName(string className)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.Name == className)
                    {
                        return type;
                    }
                }
            }
            return null;
        }

        private static void getViewContent(GameObject prefab, out string viewCode, out string presenterCode)
        {
            viewCode =
                "using System;\n" +
                "using System.Collections;\n" +
                "using System.Collections.Generic;\n" +
                "using UnityEngine;\n" +
                "using UnityEngine.UI;\n" +
                "using SF;\n\n";
            presenterCode = viewCode;
            string viewName = prefab.name.Substring(2);
            viewCode += "public class SF" + viewName + "View : SFBaseView\n{\n";
            string viewPart1 = "";
            string viewPart2 = "";
            string viewPart3 = "";
            presenterCode += "namespace SF\n{\n    public class SF" + viewName + "Presenter\n" +
                "    {\n" +
                "        SF" + viewName + "View m_view;\n" +
                "        public void initWithView(SF" + viewName + "View view)\n" +
                "        {\n" +
                "            m_view = view;\n\n";
            string presenterPart1 = "";
            string presenterPart2 = "";
            foreach (RectTransform trans in prefab.GetComponentsInChildren<RectTransform>())
            {
                var GO = trans.gameObject;
                string prefix = GO.name.Substring(0, 3);
                if (prefix == "lbl")
                {
                    // Text
                    viewPart1 += "    public Text " + GO.name + " { get { return m_" + GO.name + "; } }\n";
                    viewPart2 += "    private Text m_" + GO.name + ";\n";
                    viewPart3 +=
                        "        GameObject " + GO.name + "GO = SFUtils.findChildWithParent(gameObject, \"" + GO.name + "\");\n" +
                        "        if (" + GO.name + "GO != null)\n" +
                        "        {\n" +
                        "            m_" + GO.name + " = " + GO.name + "GO.GetComponent<Text>();\n" +
                        "        }\n\n";
                }
                else if (prefix == "btn")
                {
                    // Button
                    viewPart1 += "    public Button " + GO.name + " { get { return m_" + GO.name + "; } }\n";
                    viewPart2 += "    private Button m_" + GO.name + ";\n";
                    viewPart3 +=
                        "        GameObject " + GO.name + "GO = SFUtils.findChildWithParent(gameObject, \"" + GO.name + "\");\n" +
                        "        if (" + GO.name + "GO != null)\n" +
                        "        {\n" +
                        "            m_" + GO.name + " = " + GO.name + "GO.GetComponent<Button>();\n" +
                        "        }\n\n";
                    presenterPart1 += "            m_view.addEventListener(m_view." + GO.name + ", SFEvent.EVENT_UI_CLICK, on" + GO.name.Substring(3) + ");\n";
                    presenterPart2 += "\n" +
                        "        void on" + GO.name.Substring(3) + "(SFEvent e)\n" +
                        "        {\n" +
                        "        }\n";
                }
                else if (prefix == "tgb")
                {
                    // Toggle
                }
                else if (prefix == "img")
                {
                    // Image
                }
            }
            viewCode += viewPart1 + "\n" + viewPart2 + "\n" + "    private SF" + viewName + "Presenter m_presenter;\n\n" +
                "    void Start()\n{\n" +
                "#if UNITY_EDITOR\n" +
                "        var time1 = DateTime.Now;\n" +
                "#endif\n";
            viewCode += viewPart3 +
                "        m_presenter = new SF" + viewName + "Presenter();\n" +
                "        m_presenter.initWithView(this);\n\n" +
                "#if UNITY_EDITOR\n" +
                "        var time2 = DateTime.Now;\n" +
                "        var diff = time2.Subtract(time1);\n" +
                "        SFUtils.log(string.Format(\"View created: vw" + viewName + ", cost {0}ms\", diff.TotalMilliseconds));\n" +
                "#else\n" +
                "        SFUtils.log(\"View created: vw" + viewName + "\");\n" +
                "#endif\n" +
                "    }\n}\n";
            presenterCode += presenterPart1 + "        }\n" + presenterPart2 + "    }\n}\n";
        }
    }
}
