using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static LevelEditorCommands;

namespace Core.Scripts.LevelEditor
{
    public class LoadCommand : AbstractLevelEditorCommand<Load>
    {
        public LoadCommand(LevelEditorPresenter presenter) : base(presenter) { }

        public override void Execute(Load arg) => view.StartCoroutine(LoadAsset());

        private IEnumerator LoadAsset()
        {
            var handle = Addressables.LoadAssetsAsync<LevelDataSO>(model.ModelSO.DataLevelsReference, null);

            yield return handle;

#if UNITY_EDITOR
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                List<LevelDataSO> result = new List<LevelDataSO>(handle.Result);
                EditorPopupLevelSelect.ShowWindow(result, OnLoad, OnClose);
                presenter.InvokeAnticlickSetActive(true);
                presenter.LockInput(true);
            }
#endif
            Addressables.Release(handle);
        }

        private void OnLoad(LevelDataSO data)
        {
            if (data == null)
                return;

            LayersData<int> layersData = data.LayersData;
            NewCell oldCell;
            int totalCounter = 0;

            for (int layerIndex = 0; layerIndex < layersData.Layers.Count; layerIndex++)
            {
                List<int> newLayerData = layersData[layerIndex];
                IEnumerator<NewCell> enumerator = model.GetLayer(layerIndex).GetEnumerator();

                for (int cellIndex = 0; cellIndex < newLayerData.Count; cellIndex++)
                {
                    enumerator.MoveNext();
                    oldCell = enumerator.Current;

                    bool needTile = newLayerData[cellIndex] != 0;

                    if(oldCell.HaveTile)
                    {
                        if (needTile)
                            totalCounter++;
                        else
                        {
                            if (oldCell.Tile != null)
                                presenter.TileFactory.Release(oldCell.Tile);

                            oldCell.RemoveTile();
                        }
                    }
                    else if(needTile)
                    {
                        totalCounter++;
                        oldCell.MarkHaveTile();
                    }
                }
            }

            model.SetNewLevelData(data, totalCounter);
            presenter.ViewClickLayer(0);
            presenter.ViewDisplayVersion();
        }
        private void OnClose()
        {
            presenter.InvokeAnticlickSetActive(false);
            presenter.LockInput(false);
        }
    }
}