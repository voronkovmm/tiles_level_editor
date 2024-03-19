using System.Collections.Generic;
using UnityEngine;
using static LevelEditorCommands;

namespace Core.Scripts.LevelEditor
{
    public class TransparentLayer : AbstractLevelEditorCommand<Transparent>
    {
        private Color tileDefaultColor;
        private Color tileTransparentColor;
        private bool isEnable;
        private bool isSubscribed;

        private List<Tile> tiles = new(50);

        public TransparentLayer(LevelEditorPresenter presenter) : base(presenter) 
        {
            tileDefaultColor = model.ModelSO.TileColor;
            tileTransparentColor = model.ModelSO.TransparentTileColor;
        }

        public override void Execute(Transparent arg)
        {
            isEnable = !isEnable;

            if (!isEnable)
            {
                Disable();
                return;
            }

            if (!isSubscribed)
            {
                isSubscribed = true;
                model.OnSwitchLayer += Show;
            }

            Show(model.CurrentLayerId);
        }

        private void Show(int currentLayer)
        {
            if (model.CurrentLayerId == 0)
            {
                ReleaseAll();
                return;
            }

            NewGrid<NewCell> botLayer = model.GetLayer(model.CurrentLayerId - 1);
            NewGrid<NewCell> topLayer = model.GetLayer(currentLayer);

            IEnumerator<NewCell> botEnumerator = botLayer.GetEnumerator();
            IEnumerator<NewCell> topEnumerator = topLayer.GetEnumerator();

            NewCell botCell;
            NewCell topCell;

            ReleaseAll();

            while (botEnumerator.MoveNext())
            {
                if (!topEnumerator.MoveNext())
                    break;

                botCell = botEnumerator.Current;
                topCell = topEnumerator.Current;

                if (botCell.HaveTile && !topCell.HaveTile)
                {
                    Tile transparentTile = presenter.TileFactory.Get(topCell);
                    tiles.Add(transparentTile);
                    transparentTile.transform.SetAsFirstSibling();
                    transparentTile.Color = tileTransparentColor;
                }
            }
        }

        private void ReleaseAll()
        {
            foreach (var tile in tiles)
            {
                tile.Color = tileDefaultColor;
                presenter.TileFactory.Release(tile);
            }
            tiles.Clear();
        }

        private void Disable()
        {
            ReleaseAll();

            model.OnSwitchLayer -= Show;
            isSubscribed = false;
        }
    } 
}