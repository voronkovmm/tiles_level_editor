using Core.Scripts.LevelEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Scripts
{
    // TODO: halfSpacing поменять везде где spacing делится
    public class NewGrid<TCell> where TCell : NewCell
    {
        private int layer;
        private int width;
        private int height;
        private int widthInner;
        private int heightInner;
        private Vector2 spacing;
        private Vector2 cellSize;
        private float offsetDetectInnerCell;
        private Vector2 cellSizeInner;
        private Camera camera;
        private Vector2 startPosition;
        private RectTransform rect;
        private LevelEditorModelSO modelSO;
        private TCell[,] cells;

        public int TotalWidth => widthInner;
        public int TotalHeight => heightInner;
        public int Layer => layer;
        public Vector2 CellSize => cellSize;

        public NewGrid(int layer, LevelEditorModelSO model, Vector2 cellSize, RectTransform rect, Func<int, int, NewGrid<TCell>, TCell> createFunc)
        {
            this.rect          = rect;
            this.width         = model.GridSize.x;
            this.height        = model.GridSize.y;
            this.widthInner    = width * 2 - 1;
            this.heightInner   = height * 2 - 1;
            this.spacing       = model.Spacing;
            this.cellSize      = cellSize;
            this.cellSizeInner = cellSize / 2;
            this.offsetDetectInnerCell = model.OffsetDetectInnerCell;

            this.layer = layer;
            this.modelSO = model;
            camera = Camera.main;

            startPosition = Voron.Voron.TransformToAnchor(rect.anchoredPosition, new Vector2(rect.anchorMin.x, rect.anchorMin.y), (RectTransform)rect.parent, Voron.Voron.EnumAnchor.CENTER);

            CreateGrid(createFunc);
        }

        public bool TryGetCell(int x, int y, out TCell cell)
        {
            cell = null;

            if (x >= 0 && x < widthInner && y >= 0 && y < heightInner)
            {
                cell = cells[x, y];
                return true;
            }

            return false;
        }
        public bool TryGetCell(Vector2Int index, out TCell cell) => TryGetCell(index.x, index.y, out cell);
        public bool TryGetCell(Vector3 mousePos, out TCell cell)
        {
            Vector2Int index = GetIndex(mousePos);

            if (TryGetCell(index, out cell))
                return true;

            return false;
        }

        public Vector2 ScreenPointToGridPoint(Vector3 screenPoint)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPoint, camera, out Vector2 gridLocalPoint);
            return gridLocalPoint;
        }
        
        public Vector2 GetCellLocalPosition(NewCell cell)
        {
            Vector2 pos = new()
            {
                x = cell.X * (cellSizeInner.x + spacing.x/2),
                y = cell.Y * (cellSizeInner.y + spacing.y/2)
            };

            Vector2 offset = .5f * CellSize;
            pos += offset;

            return pos;
        }
        public Vector2 GetCellWorldPos(Vector2Int index)
        {
            if (TryGetCell(index, out TCell cell))
            {
                return rect.TransformPoint(GetCellLocalPosition(cell));
            }

            return Vector2.zero;
        }

        public Vector2Int GetIndex(Vector3 screenPosition)
        {
            Vector2 localPoint = ScreenPointToGridPoint(screenPosition);

            // сдвигаем localPoint + (spacing/2), из-за расчетов спейсинга grid. Из-за этого необходимо обнаружить клик за пределами сетки.
            if (localPoint.x < 0 || localPoint.y < 0 || localPoint.x > rect.rect.width || localPoint.y > rect.rect.height)
                return new Vector2Int(-1, -1);

            Vector2 cellSizeWithSpacing = cellSize + spacing;
            Vector2 pos = (localPoint + (spacing / 2)) / cellSizeWithSpacing;

            Vector2Int index = new()
            {
                x = Mathf.FloorToInt(pos.x),
                y = Mathf.FloorToInt(pos.y)
            };

            Vector2Int indexInner = index * 2;

            // обнаружение попадания во внутреннюю ячейку
            float offsetMin = offsetDetectInnerCell;
            float offsetMax = 1 - offsetDetectInnerCell;
            
            float xDecimal = Mathf.Abs(pos.x - index.x);
            if (pos.x > offsetMin && pos.x <= width - 1 + offsetMax && (xDecimal < offsetMin || xDecimal > offsetMax))
                indexInner.x += xDecimal < offsetMin ? -1 : 1;

            float yDecimal = Mathf.Abs(pos.y - index.y);
            if (pos.y > offsetMin && pos.y <= height - 1 + offsetMax && (yDecimal < offsetMin || yDecimal > offsetMax))
                indexInner.y += yDecimal < offsetMin ? -1 : 1;


            if (modelSO.DebugEnable)
            {
                Debug.Log($"<color=red>x={indexInner.x}, y={indexInner.y}");
                Vector2 worldPos = GetCellWorldPos(indexInner);
                Debug.DrawLine(Vector3.zero, worldPos, Color.blue, 0.1f); 
            }

            return indexInner;
        }

        public IEnumerator<TCell> GetEnumerator()
        {
            for (int x = 0; x < widthInner; x++)
                for (int y = 0; y < heightInner; y++)
                    yield return cells[x, y];
        }
        public IEnumerable<TCell> GetEnumerable()
        {
            for (int x = 0; x < widthInner; x++)
                for (int y = 0; y < heightInner; y++)
                    yield return cells[x, y];
        }
        public IEnumerable<TCell> GetNeighbors(TCell cell)
        {
            int xIndex = cell.X;
            int yIndex = cell.Y;

            for (int x = xIndex - 1; x < xIndex + 2; x++)
                for (int y = yIndex - 1; y < yIndex + 2; y++)
                {
                    if (x == xIndex && y == yIndex)
                        continue;

                    if (TryGetCell(x, y, out TCell neighbor))
                        yield return neighbor;
                }
        }

        private void CreateGrid(Func<int, int, NewGrid<TCell>, TCell> createFunc)
        {
            cells = new TCell[widthInner, heightInner];

            for (int x = 0; x < widthInner; x++)
                for (int y = 0; y < heightInner; y++)
                {
                    cells[x, y] = createFunc(x, y, this);
                }

            foreach (var cell in cells)
                cell.RefreshNeighbours();
        }
    }
}