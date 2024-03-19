using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Scripts.LevelEditor
{
    public class LevelEditorModel
    {
        public const int TOTAL_LAYERS = 5;
        public readonly string PATH_TO_SAVE = "Assets/Core/Bundles/DataLevels/";
        public readonly string FORMAT_NAME_TO_SAVE = "{0}.asset";

        public event Action<bool> OnPreviewStateChange;
        public event Action<bool> OnValidateStateChange;
        public event Action<int, int> OnAddTile;
        public event Action<int, int> OnRemoveTile;
        public event Action<int> OnSwitchLayer;

        public int PrevLayerId { get; private set; }
        public int CurrentLayerId { get; private set; }
        public int TileTotalCounter { get; private set; }
        public int TileLayerCounter { get; private set; }
        public bool IsPreviewActive { get; private set; }
        public bool IsValidateActive { get; private set; }
        public bool IsDirty { get; private set; }
        public NewGrid<NewCell> Grid;
        public readonly LevelEditorModelSO ModelSO;

        private readonly Vector2 cellSize;
        private List<NewGrid<NewCell>> layers = new(TOTAL_LAYERS);
        private readonly RectTransform gridRect;
        private readonly ViewLevelEditor view;
        private readonly LevelEditorPresenter presenter;
        private LevelDataSO levelData;

        public int LayersCount => layers.Count;
        public string LevelVersion => levelData ? levelData.Version : ModelSO.Version;

        public LevelEditorModel(ViewLevelEditor view, LevelEditorPresenter presenter, LevelEditorModelSO modelSO, RectTransform gridRect, Vector2 cellsize)
        {
            this.view      = view;
            this.presenter = presenter;
            this.ModelSO   = modelSO;
            this.gridRect  = gridRect;
            this.cellSize  = cellsize;
            CreateGrids();
        }

        public void SwitchLayer(int index)
        {
            PrevLayerId = Grid.Layer;
            CurrentLayerId = index;
            Grid = layers[index];
            OnSwitchLayer?.Invoke(index);
        }

        public NewGrid<NewCell> GetLayer(int index) => layers[index];
        public List<NewGrid<NewCell>> GetLayers() => layers;
        public string GetFileNameWithExtension(string name) => string.Format(FORMAT_NAME_TO_SAVE, name);

        public void SetValidateActive(bool value)
        {
            IsValidateActive = value;
            OnValidateStateChange?.Invoke(value);
        }
        public void SetPreviewActive(bool value)
        {
            IsPreviewActive = value;
            OnPreviewStateChange?.Invoke(value);
        }
        public void SetNewLevelData(LevelDataSO data, int totalCounter)
        {
            TileTotalCounter = totalCounter;
            levelData = data;
            IsDirty = true;
        }

        public void ResetDirty() => IsDirty = false;

        public void AddTile()
        {
            TileLayerCounter++;
            TileTotalCounter++;
            view.RefreshTileCounters(TileLayerCounter, TileTotalCounter);
        }

        public void RemoveTile()
        {
            TileLayerCounter--;
            TileTotalCounter--;
            view.RefreshTileCounters(TileLayerCounter, TileTotalCounter);
        }

        private void CreateGrids()
        {
            for (int i = 0; i < TOTAL_LAYERS; i++)
                layers.Add(new(i, ModelSO, cellSize, gridRect, (x, y, grid) => new NewCell(x, y, grid)));

            Grid = layers[0];
            CurrentLayerId = 0;
        }

        public void RefreshTileLayerCounter(int tilesCounter)
        {
            TileLayerCounter = tilesCounter;
            view.RefreshTileCounters(TileLayerCounter, TileTotalCounter);
        }
        public void RefreshTileTotalCounter(int tilesCounter)
        {
            TileTotalCounter = tilesCounter;
            view.RefreshTileCounters(TileLayerCounter, TileTotalCounter);
        }
        public void RefreshTileCounters(int layerCounter, int totalCounter)
        {
            TileLayerCounter = layerCounter;
            TileTotalCounter = totalCounter;
            view.RefreshTileCounters(TileLayerCounter, TileTotalCounter);
        }
    } 
}