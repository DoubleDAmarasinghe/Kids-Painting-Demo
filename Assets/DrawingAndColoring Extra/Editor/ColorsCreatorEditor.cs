using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using UnityEditor.SceneManagement;
using IndieStudio.DrawingAndColoring.Utility;
using IndieStudio.DrawingAndColoring.Logic;
using Tool = IndieStudio.DrawingAndColoring.Logic.Tool;
using System.Linq;

///Developed by Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com


public class ColorsCreatorEditor : EditorWindow
{
    private Vector2 scrollPos;
    private Vector2 scale;
    private Vector2 scrollView = new Vector2(550, 430);
    private static ColorsCreatorEditor window;
    private static readonly int size = 7;
    private static GameObject[] contentPrefabs = new GameObject[size];
    private static Tool[] tools = new Tool[size];
    private static string[] staticPaths = new string[] {
            "Assets/DrawingAndColoring Extra/Prefabs/Crayons/",
            "Assets/DrawingAndColoring Extra/Prefabs/Pencils/",
            "Assets/DrawingAndColoring Extra/Prefabs/PaintRoller/",
            "Assets/DrawingAndColoring Extra/Prefabs/Sparkles/",
            "Assets/DrawingAndColoring Extra/Prefabs/Paints/",
            "Assets/DrawingAndColoring Extra/Prefabs/WaterBrushes/",
            "Assets/DrawingAndColoring Extra/Prefabs/PaintingCan/"
        };

    private bool  deleteAllContents = false;

    private static List<Color> ColorsList = new List<Color>();

    //TODO or uncomment to activate (Do not forget to put ToolConents prefabs in Resources folder with valid name based on code)
    //[MenuItem("Tools/Drawing And Coloring Extra/Game Scene/Colors Factory #f", false, 0)]
    static void ShapesFactoryManager()
    {
        Init();
    }

    //[MenuItem("Tools/Drawing And Coloring Extra/Game Scene/Colors Factory #f", true, 0)]
    static bool ShapesFactoryManagerValidate()
    {
        return !Application.isPlaying && SceneManager.GetActiveScene().name == "Game";
    }

    public static void Init()
    {
        contentPrefabs[0] = Resources.Load("Circles/Crayon", typeof(GameObject)) as GameObject;
        contentPrefabs[1] = Resources.Load("Circles/Pencil", typeof(GameObject)) as GameObject;
        contentPrefabs[2] = Resources.Load("Circles/PaintRoller", typeof(GameObject)) as GameObject;
        contentPrefabs[3] = Resources.Load("Circles/Sparkle", typeof(GameObject)) as GameObject;
        contentPrefabs[4] = Resources.Load("Circles/Paint", typeof(GameObject)) as GameObject;
        contentPrefabs[5] = Resources.Load("Circles/WaterBrush", typeof(GameObject)) as GameObject;
        contentPrefabs[6] = Resources.Load("Circles/PaintingCan", typeof(GameObject)) as GameObject;


        tools[0] = GameObject.Find("CrayonTool").GetComponent<Tool>();
        tools[1] = GameObject.Find("PencilTool").GetComponent<Tool>();
        tools[2] = GameObject.Find("PaintRollerTool").GetComponent<Tool>();
        tools[3] = GameObject.Find("SparkleTool").GetComponent<Tool>();
        tools[4] = GameObject.Find("PaintTool").GetComponent<Tool>();
        tools[5] = GameObject.Find("WaterColorTool").GetComponent<Tool>();
        tools[6] = GameObject.Find("PaintingCanTool").GetComponent<Tool>();

        window = (ColorsCreatorEditor)EditorWindow.GetWindow(typeof(ColorsCreatorEditor));
        float windowSize = Screen.currentResolution.height * 0.75f;
        window.position = new Rect(50, 100, windowSize, windowSize);
        window.maximized = false;
        window.titleContent.text = "Shape Factory";
        window.Show();
    }

    void OnGUI()
    {
        if (window == null || Application.isPlaying)
        {
            return;
        }

        window.Repaint();

        scrollView = new Vector2(position.width, position.height);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(scrollView.x), GUILayout.Height(scrollView.y));


        EditorGUILayout.Separator();
        for (int i = 0; i < size; i++)
        {
            EditorGUILayout.TextField("Path " + (i + 1), staticPaths[i]);
        }

        for (int i = 0; i < size; i++)
        {
            EditorGUILayout.ObjectField("Content " + (i + 1), contentPrefabs[i], typeof(GameObject), allowSceneObjects: false);
        }

        for (int i = 0; i < size; i++)
        {
            EditorGUILayout.ObjectField("Tool " + (i + 1), tools[i], typeof(Tool), allowSceneObjects: true);
        }
        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();

        GUI.backgroundColor = Colors.greenColor;
        if (GUILayout.Button("+", GUILayout.Width(30), GUILayout.Height(30)))
        {

            ColorsList.Add(Color.white);
        }
        GUI.backgroundColor = Colors.whiteColor;

        GUI.backgroundColor = Colors.redColor;
        if (GUILayout.Button("-", GUILayout.Width(30), GUILayout.Height(30)))
        {
            if (ColorsList.Count > 0)
            {
                ColorsList.RemoveAt(ColorsList.Count - 1);
            }
        }
        GUI.backgroundColor = Colors.whiteColor;

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));


        deleteAllContents = EditorGUILayout.Toggle("Clear Tool Contents", deleteAllContents);

       EditorGUILayout.Separator();

        GUI.backgroundColor = Colors.greenColor;

        if (GUILayout.Button("Append Colors Tools", GUILayout.ExpandWidth(true), GUILayout.Height(25)))
        {
            if (contentPrefabs.ToList().Where(x => x == null).Any() || tools.ToList().Where(x => x == null).Any())
            {
                EditorUtility.DisplayDialog("", "", @"Plese select Circles prefabs from path '\DrawingAndColoring Extra\Prefabs\Circles' and the Tools from Game scene", "ok");

                return;
            }

            bool isOk = EditorUtility.DisplayDialog("Confirm Message", "Are you sure that you want to perform the process ?", "yes", "no");
            if (!isOk) return;

            for (int i = 0; i < size; i++)
            {
                if (deleteAllContents)
                {
                    tools[i].contents.Clear();
                }

                for(int j = 0; j < ColorsList.Count;j++)
                {
                    var c = ColorsList[j];

                    //TODO add Image for each color e.g for Pencil/Crayon defined by user input

                    GameObject intance = Instantiate(contentPrefabs[i], Vector3.zero, Quaternion.identity);
                    intance.transform.position = Vector3.zero;

                    ToolContent contentComponent = intance.GetComponent<ToolContent>();
                    Image imageComponent = intance.GetComponent<Image>();

                    imageComponent.color = c;

                    Gradient gradient = new Gradient();
                    var colorKey = new GradientColorKey[2];
                    colorKey[0].color = c;
                    colorKey[0].time = 0.0f;
                    colorKey[1].color = c;
                    colorKey[1].time = 1.0f;

                    var alphaKey = new GradientAlphaKey[2];
                    alphaKey[0].alpha = tools[i].name == "PaintRollerTool" ? 0.5f : 1.0f;
                    alphaKey[0].time = 0.0f;
                    alphaKey[1].alpha = tools[i].name == "PaintRollerTool" ? 0.5f : 1.0f;
                    alphaKey[1].time = 1.0f;

                    gradient.SetKeys(colorKey, alphaKey);

                    contentComponent.gradientColor = gradient;

                    var pathToSave = staticPaths[i] + contentPrefabs[i].name +(j+1) + ".prefab";

                    GameObject savedShapePrefab = CommonUtil.SaveAsPrefab(pathToSave, intance, false);

                    tools[i].contents.Add(savedShapePrefab.transform);

                    PrefabUtility.ApplyPrefabInstance(tools[i].gameObject, InteractionMode.AutomatedAction);

                    DestroyImmediate(intance);
                }
            }

        }
        GUI.backgroundColor = Colors.whiteColor;

        EditorGUILayout.LabelField("You have added (" + ColorsList.Count + ")" + " colors", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical();
        for (int i = 0; i < ColorsList.Count; i++)
        {
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Circle Color [" + i + "]", EditorStyles.boldLabel);
            EditorGUILayout.Separator();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            ColorsList[i] = EditorGUILayout.ColorField("Color", ColorsList[i]);
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();



        EditorGUILayout.EndScrollView();
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
}