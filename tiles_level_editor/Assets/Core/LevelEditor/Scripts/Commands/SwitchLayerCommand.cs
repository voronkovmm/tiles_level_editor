using System.Collections.Generic;
using System.Linq;
using static LevelEditorCommands;

namespace Core.Scripts.LevelEditor
{
    public class SwitchLayerCommand : AbstractLevelEditorCommand<Switch>
    {
        private int tilesCounter;
        private int newLayer;
        private int prevLayer;

        public SwitchLayerCommand(LevelEditorPresenter presenter) : base(presenter) { }

        public override void Execute(Switch arg)
        {
            newLayer = arg.ID;
            model.SwitchLayer(newLayer);
            prevLayer = model.PrevLayerId;

            Switch();
            SetLayersColor();
            model.RefreshTileLayerCounter(tilesCounter);
        }

        private void Switch()
		{
            if (model.IsDirty)
                model.ResetDirty();
            else if (newLayer == prevLayer)
                return;

            if (model.IsPreviewActive)
            {
                view.TweenScalePreviewBtn();
                return;
            }

            NewGrid<NewCell> oldGrid = model.GetLayer(prevLayer);
            NewGrid<NewCell> newGrid = model.GetLayer(newLayer);

            IEnumerator<NewCell> enumeratorNew = newGrid.GetEnumerator();
            IEnumerator<NewCell> enumeratorOld = oldGrid.GetEnumerator();

            bool isSameLayer = oldGrid.Layer == newGrid.Layer;

            tilesCounter = 0;
            NewCell newCell;
            NewCell oldCell;
            while (enumeratorNew.MoveNext() && enumeratorOld.MoveNext())
            {
                newCell = enumeratorNew.Current;
                oldCell = enumeratorOld.Current;

                if (oldCell.HaveTile)
                {
                    if (newCell.HaveTile)
                    {
                        if (isSameLayer)
                        {
                            if (newCell.Tile == null)
                            {
                                Tile tile = presenter.TileFactory.Get(newCell);
                                newCell.ReplaceTile(tile);
                            }
                            tilesCounter++;
                        }
                        else if (!isSameLayer)
                        {
                            newCell.ReplaceTile(oldCell.Tile);
                            oldCell.ClearTileLink();
                            tilesCounter++;
                        }
                    }
                    else if(oldCell.Tile != null)
                    {
                        presenter.TileFactory.Release(oldCell.Tile);
                        oldCell.ClearTileLink();
                    }
                }
                else if(newCell.HaveTile)
                {
                    Tile tile = presenter.TileFactory.Get(newCell);
                    newCell.ReplaceTile(tile);
                    tilesCounter++;
                }
            }
        }

        private void SetLayersColor()
        {
            for (int i = 0; i < model.LayersCount; i++)
            {
                if (i == newLayer)
                {
                    presenter.View.SetLayerColor(newLayer, ViewLevelEditor.EnumLayerColor.ACTIVE);
                    continue;
                }

                NewGrid<NewCell> grid = model.GetLayer(i);

                if (grid.GetEnumerable().Any(x => x.HaveTile))
                    presenter.View.SetLayerColor(i, ViewLevelEditor.EnumLayerColor.NOT_EMPTY);
                else
                    presenter.View.SetLayerColor(i, ViewLevelEditor.EnumLayerColor.EMPTY);
            }
        }
    } 
}
