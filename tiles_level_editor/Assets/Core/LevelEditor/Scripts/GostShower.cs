using Core.Scripts.LevelEditor;
using UnityEngine;

namespace Core.Scripts
{
    public class GostShower
	{
        private Gost gost;
        private Vector2Int prevIndex;
        private NewCell cell;

        private ViewLevelEditor view;
        private LevelEditorModel model;
        private LevelEditorPresenter presenter;

        public GostShower(LevelEditorPresenter presenter)
        {
            this.presenter = presenter;
            this.view = presenter.View;
            this.model = presenter.Model;

            gost = UnityEngine.Object.Instantiate(model.ModelSO.GostPrefab);
            gost.Initialize(model.Grid.CellSize, view.transform as RectTransform);
        }

        public void Show()
        {
            Vector3 mousePos = Input.mousePosition;
            Vector2Int index = model.Grid.GetIndex(mousePos);

            if (index == prevIndex || !model.Grid.TryGetCell(index, out cell) || cell.HaveTile)
                return;

            prevIndex = index;
            if (cell.NeighboursHaveTile)
            {
                gost.Enable = false;
                return;
            }
            else
            {
                gost.Enable = true;
                Vector2 worldPos = model.Grid.GetCellWorldPos(index);
                gost.transform.position = worldPos;
            }
        }
    } 
}

