#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TagExplorer : EditorWindow
{
    [MenuItem("Window/Tag Explorer")]
    static void Init()
    {
        TagExplorer window = (TagExplorer)GetWindow(typeof(TagExplorer));
        window.Show();
    }

    private Vector2 _scrollPosition;
    private GameObject[] allGameObjects;
    private void OnGUI()
    {

        if (GUILayout.Button("Refresh"))
        {
            OnEnable();
        }

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        for (int i = 0; i < allGameObjects.Length; i++)
        {
            GUIStyle style = new GUIStyle();
            style.normal.background = i % 2 == 0 ? _darkTex : _lightTex;

            GameObject obj = allGameObjects[i];

            if (obj == null) continue;

            EditorGUILayout.BeginHorizontal(style);
            EditorGUILayout.LabelField(obj.name);

            var lastRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseDown && lastRect.Contains(Event.current.mousePosition))
            {
                Selection.activeGameObject = obj;
            }

            EditorGUI.BeginChangeCheck();
            string tag = EditorGUILayout.TagField(obj.tag);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(obj, "TagChange");
                obj.tag = tag;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    void OnEnable()
    {
        _darkTex = MakeTex(600, 1, Color.white * 0.1f);
        _lightTex = MakeTex(600, 1, Color.white * 0.3f);
        allGameObjects = GetAllObjectsOnlyInScene().Where(x => x.tag != "Untagged").ToArray();
    }

    private static Texture2D _darkTex;
    private static Texture2D _lightTex;


    // source http://forum.unity3d.com/threads/20510-Giving-UnityGUI-elements-a-background-color.?p=430604#post430604
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }

    private static List<GameObject> GetAllObjectsOnlyInScene()
    {
        List<GameObject> objectsInScene = new List<GameObject>();

        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (!EditorUtility.IsPersistent(go.transform.root.gameObject) && !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave))
                objectsInScene.Add(go);
        }

        return objectsInScene;
    }
}
#endif
