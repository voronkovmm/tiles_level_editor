using System.Collections.Generic;
using static LevelEditorCommands;

namespace Core.Scripts.LevelEditor
{
    public class CopyBottomLayerCommand : AbstractLevelEditorCommand<Copy>
    {
        public CopyBottomLayerCommand(LevelEditorPresenter presenter) : base(presenter) { }

        public override void Execute(Copy arg)
        {
            if (model.CurrentLayerId == 0)
                return;

            if (model.IsPreviewActive)
            {
                view.TweenScalePreviewBtn();
                return;
            }

            int layerId = model.CurrentLayerId;
            NewGrid<NewCell> currentLayer = model.GetLayer(layerId);
            NewGrid<NewCell> bottomLayer = model.GetLayer(layerId - 1);

            IEnumerator<NewCell> enumeratorCurrent = currentLayer.GetEnumerator();
            IEnumerator<NewCell> enumeratorBottom = bottomLayer.GetEnumerator();

            int differenceCounter = 0;
            NewCell currentCell;
            NewCell botCell;
            while (enumeratorCurrent.MoveNext() && enumeratorBottom.MoveNext())
            {
                currentCell = enumeratorCurrent.Current;
                botCell = enumeratorBottom.Current;

                if (botCell.HaveTile)
                {
                    if (!currentCell.HaveTile)
                    {
                        Tile tile = presenter.TileFactory.Get(currentCell);
                        currentCell.ReplaceTile(tile);
                        differenceCounter++;
                    }
                }
                else
                {
                    if (currentCell.HaveTile)
                    {
                        presenter.TileFactory.Release(currentCell.Tile);
                        currentCell.RemoveTile();
                        differenceCounter--;
                    }
                }
            }

            int layerCounter = model.TileLayerCounter + differenceCounter;
            int totalCounter = model.TileTotalCounter + differenceCounter;
            model.RefreshTileCounters(layerCounter, totalCounter);
        }
    }
}