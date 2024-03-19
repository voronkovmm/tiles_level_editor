using Core.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewCell
{
    private readonly int x;
    private readonly int y;
    private readonly NewGrid<NewCell> grid;

    private List<NewCell> neighbours = new(8);

    public int X => x;
    public int Y => y;
    public bool HaveTile { get; private set; }
    public Tile Tile { get; private set; }

    public NewCell(int x, int y, NewGrid<NewCell> grid)
    {
        this.x = x;
        this.y = y;
        this.grid = grid;
    }

    public bool TrySetTile(Func<Tile> createTile)
    {
        if (HaveTile)
            throw new System.Exception($"попытка установить tile в занятую ячейку x={x}, y={y}");

        if (HaveTile || NeighboursHaveTile)
            return false;
        
        Tile = createTile();
        HaveTile = true;
        return true;
    }
    public void MarkHaveTile() => HaveTile = true;
    public void RemoveTile()
    {
        HaveTile = false;
        Tile = null;
    }
    public void ReplaceTile(Tile tile)
    {
        Tile = tile;

        if (tile != null)
            MarkHaveTile();
    }

    public void ClearTileLink() => Tile = null;

    public bool NeighboursHaveTile
    {
        get
        {
            bool value = false;
            try
            {
                value = neighbours.Any(x => x.HaveTile);
            }
            catch
            {
                Debug.Log("df");
            }
            return value;
        }
    }

    public Vector2 GetLocalPosition() => grid.GetCellLocalPosition(this);

    public void RefreshNeighbours()
    {
        neighbours.Clear();
        neighbours.AddRange(grid.GetNeighbors(this));
        neighbours.RemoveAll(x => x == null);
    }
}
