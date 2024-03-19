using Core.Scripts;
using System.Collections.Generic;
using UnityEngine;
using System;
using Voron;
using System.Linq;
using System.IO;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelDataSO : ScriptableObject
{
    [Space()]
    [ReadOnly, SerializeField] private string Level;
    [ReadOnly, SerializeField] private string version;
    [SerializeField, HideInInspector] private LayersData<int> layersData;
    [SerializeField, HideInInspector] public byte[] screenshotData;

    public string Name => name;
    public string Version => version;
    public LayersData<int> LayersData => layersData;

    public void SetNewData(string version, List<NewGrid<NewCell>> layers)
    {
#if UNITY_EDITOR
        this.version = version;
        this.Level = name;
        layersData = CreateLayersData(layers);
        screenshotData = Voron.Voron.TakeScreenshot(Camera.main, 240, 135, 40);
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }

    public LayersData<int> CreateLayersData(List<NewGrid<NewCell>> layers)
    {
        int totalCells = layers[0].TotalWidth * layers[0].TotalHeight;

        LayersData<int> newData = new LayersData<int>(layers.Count, totalCells);

        for (int layerIndex = 0; layerIndex < layers.Count; layerIndex++)
        {
            IEnumerator<NewCell> enumerator = layers[layerIndex].GetEnumerator();

            for (int cellIndex = 0; cellIndex < totalCells; cellIndex++)
            {
                enumerator.MoveNext();
                newData[layerIndex].Add(enumerator.Current.HaveTile ? 1 : 0);
            }
        }

        return newData;
    }

#if UNITY_EDITOR
    public static void CreateScriptableObject(string path, List<NewGrid<NewCell>> layers, string version, string name)
    {
        LevelDataSO data = CreateInstance<LevelDataSO>();
        data.name    = name;
        data.Level   = name;
        data.version = version;
        data.layersData = data.CreateLayersData(layers);
        data.screenshotData = Voron.Voron.TakeScreenshot(Camera.main, 240, 135, 40);

        AssetDatabase.CreateAsset(data, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void OnEnable()
    {
        Level = name;   
    }
#endif
}

[Serializable]
public class LayersData<T>
{
    public List<ListWrapper<T>> Layers;

    public List<T> this[int index]
    {
        get => Layers[index].List;
        set => Layers[index].List = value;
    }

    public LayersData(int layersCount, int tilesCount)
    {
        Layers = new(layersCount);
        Layers.AddRange(Enumerable.Range(0, layersCount).Select(x => new ListWrapper<T>(tilesCount)));
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LevelDataSO))]
public class LevelDataSOEditor : Editor
{
    private LevelDataSO so;
    private byte[] textureData;
    private Texture2D screenshotTexture;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        if (screenshotTexture != null)
        {
            Rect textureRect = GUILayoutUtility.GetRect(200, 200, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUI.DrawPreviewTexture(textureRect, screenshotTexture, null, ScaleMode.ScaleToFit);
        }
    }

    private void OnValidate()
    {
        DestroyImmediate(screenshotTexture);
        CreateTexture();
    }

    private void OnEnable()
    {
        so = (LevelDataSO)target;
        textureData = so.screenshotData;
        CreateTexture();
    }

    private void OnDisable() => DestroyImmediate(screenshotTexture);

    private void CreateTexture()
    {
        screenshotTexture = new Texture2D(2, 2);
        screenshotTexture.LoadImage(so.screenshotData);
    }
}
#endif