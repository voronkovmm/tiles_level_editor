using static LevelEditorCommands;

namespace Core.Scripts.LevelEditor
{
    public class ClearCommand : AbstractLevelEditorCommand<Clear>
    {
        public ClearCommand(LevelEditorPresenter presenter) : base(presenter) { }

        public override void Execute(Clear arg)
        {
            if (model.IsPreviewActive)
            {
                view.TweenScalePreviewBtn();
                return;
            }

            foreach (var cell in model.Grid.GetEnumerable())
            {
                if (cell.HaveTile)
                {
                    Tile tile = cell.Tile;
                    presenter.TileFactory.Release(tile);
                    cell.RemoveTile();
                }
            }

            int totalCounter = model.TileTotalCounter - model.TileLayerCounter;
            model.RefreshTileCounters(0, totalCounter);
        }
    }
}
