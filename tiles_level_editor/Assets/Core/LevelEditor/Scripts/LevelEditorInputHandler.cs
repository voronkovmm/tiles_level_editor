using System;
using UnityEngine;

namespace Core.Scripts.LevelEditor
{
    public class LevelEditorInputHandler
    {
        private readonly LevelEditorPresenter presenter;
        private readonly LevelEditorModel model;

        public event Action AddTile;
        public event Action RemoveTile;

        public LevelEditorInputHandler(LevelEditorPresenter presenter)
        {
            this.presenter = presenter;
            this.model = presenter.Model;

            InputManager.OnHoldLKM += OnPressLKM;
            InputManager.OnHoldPKM += OnPressPKM;

            InputManager.OnPressAlpha += OnPressAlpha;

            InputManager.OnPressW += OnPressW;
            InputManager.OnReleaseW += OnReleaseW;

            InputManager.OnPressQ += OnPressQ;
        }

        private void OnPressQ() => presenter.ViewTransparentLayer();

        private void OnReleaseW() => presenter.ViewClickPreview();
        private void OnPressW() => presenter.ViewClickPreview();

        private void OnPressLKM()
        {
            Vector2Int index = model.Grid.GetIndex(Input.mousePosition);

            if (!model.Grid.TryGetCell(index, out NewCell cell) || cell.HaveTile)
                return;

            Vector2 localPos = model.Grid.GetCellLocalPosition(cell);
            Func<Tile> createTile = () => presenter.TileFactory.Get(cell);
            if (cell.TrySetTile(createTile))
                AddTile?.Invoke();
        }

        private void OnPressPKM()
        {
            Vector2Int index = model.Grid.GetIndex(Input.mousePosition);

            if (!model.Grid.TryGetCell(index, out NewCell cell) || !cell.HaveTile)
                return;

            presenter.TileFactory.Release(cell.Tile);
            cell.RemoveTile();
            RemoveTile?.Invoke();
        }

        private void OnPressAlpha(int num)
        {
            if (num > 0 && num <= LevelEditorModel.TOTAL_LAYERS)
                presenter.ViewClickLayer(num - 1);
        }
    } 
}
