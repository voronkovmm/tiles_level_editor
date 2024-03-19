using UnityEngine;
using static LevelEditorCommands;

namespace Core.Scripts.LevelEditor
{
    public class PreviewCommand : AbstractLevelEditorCommand<Preview>
    {
        public PreviewCommand(LevelEditorPresenter presenter) : base(presenter) { }

        public override void Execute(Preview arg)
        {
            if (!model.IsPreviewActive)
                Show();
            else
                Hide();
        }

        private void Show()
        {
            NewGrid<NewCell> tmpGrid;
            Tile tmpTile;
            for (int i = 0; i < LevelEditorModel.TOTAL_LAYERS; i++)
            {
                tmpGrid = model.GetLayer(i);

                foreach (var cell in tmpGrid.GetEnumerable())
                {
                    if (i != model.CurrentLayerId && cell.HaveTile)
                    {
                        Tile tile = presenter.TileFactory.Get(cell);
                        cell.ReplaceTile(tile);
                    }

                    tmpTile = cell.Tile;
                    if (tmpTile != null)
                    {
                        Vector2 pos = tmpTile.AnchoredPosition + model.ModelSO.PreviewOffset * i;
                        tmpTile.SetPosition(pos);
                        tmpTile.Color = model.ModelSO.GetPreviewLayerColor(i);
                        tmpTile.transform.SetAsLastSibling();
                    }
                }
            }

            model.SetPreviewActive(true);
        }

        private void Hide()
        {
            Tile tmpTile;
            NewGrid<NewCell> tmpGrid;

            for (int i = 0; i < LevelEditorModel.TOTAL_LAYERS; i++)
            {
                tmpGrid = model.GetLayer(i);

                foreach (var cell in tmpGrid.GetEnumerable())
                {
                    tmpTile = cell.Tile;

                    if (tmpTile == null)
                        continue;

                    tmpTile.Color = model.ModelSO.TileColor;

                    if (i == model.CurrentLayerId)
                        tmpTile.SetPosition(cell.GetLocalPosition());
                    else
                    {
                        presenter.TileFactory.Release(tmpTile);
                        cell.ClearTileLink();
                    }
                }
            }

            model.SetPreviewActive(false);
        }
    }
}