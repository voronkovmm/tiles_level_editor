using Core.Scripts.LevelEditor;
using UnityEngine;
using UnityEngine.Pool;

public class TileFactory
{
    private ObjectPool<Tile> pool;
    private readonly Tile prefab;
    private readonly Vector2 cellSize;
    private readonly LevelEditorModelSO modelSO;
    private readonly RectTransform parent;

    public TileFactory(LevelEditorModelSO model, Vector2 cellSize, RectTransform parent)
    {
        this.prefab = model.TilePrefab;
        this.parent = parent;
        this.cellSize = cellSize;
        this.modelSO = model;

        pool = new(
            createFunc: () => CreateNew(),
            actionOnGet: (tile) => tile.Enable = true,
            actionOnRelease: (tile) => tile.Enable = false,
            actionOnDestroy: (tile) => Object.Destroy(tile.gameObject),
            defaultCapacity: 50
            );
    }

    public Tile Get(NewCell cell)
    {
        Tile tile = pool.Get();
        tile.SetPosition(cell.GetLocalPosition());
        return tile;
    }

    public void Release(Tile tile) => pool.Release(tile);

    private Tile CreateNew()
    {
        Tile tile = Object.Instantiate(prefab, parent);
        tile.Color = modelSO.TileColor;
        tile.SetSize(cellSize);
        return tile;
    }
}
