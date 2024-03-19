using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.Scripts.LevelEditor
{
    [CreateAssetMenu(menuName = "SO/LevelEditor/Model")]
    public class LevelEditorModelSO : ScriptableObject
    {
        private const string VERSION = "0.0.1";
        private FlexibleGridLayout gridLayoutGroupCells;

        [Header("Grid")]
        [SerializeField] private Vector2Int gridSize;
        [SerializeField] private Vector2 spacing;
        [SerializeField] private float offsetDetectInnerCell = 0.2f;
        [SerializeField] private Vector2 previewOffset;
        [SerializeField] private bool debugEnable;

        [Header("Prefabs")]
        [SerializeField] private Tile tilePrefab;
        [SerializeField] private Gost gostPrefab;

        [Header("AssetReferences")]
        [SerializeField] private AssetLabelReference dataLevelsReference;

        [Header("Colors")]
        [SerializeField] private Color activeButtonColor;
        [SerializeField] private Color lbNotEmptyColor;
        [SerializeField] private Color lbEmptyColor;
        [Space()]
        [SerializeField] private Color fontColor;
        [SerializeField] private Color totalTilesCounterMultiplyThreeColor;
        [Space()]
        [SerializeField] private Color tileColor;
        [SerializeField] private Color transparentTileColor;
        [SerializeField] private Color previewLayer_1_Color;
        [SerializeField] private Color previewLayer_2_Color;
        [SerializeField] private Color previewLayer_3_Color;
        [SerializeField] private Color previewLayer_4_Color;
        [SerializeField] private Color previewLayer_5_Color;

        [ReadOnly, Space()] public string Version = VERSION;

        public Vector2Int GridSize         => gridSize;
        public Vector2 Spacing             => spacing;
        public Vector2 PreviewOffset       => previewOffset;
        public Gost GostPrefab             => gostPrefab;
        public Tile TilePrefab             => tilePrefab;
        public bool DebugEnable            => debugEnable;
        public float OffsetDetectInnerCell => offsetDetectInnerCell;
        public Color FontColor             => fontColor;
        public Color LbNotEmptyColor       => lbNotEmptyColor;
        public Color LbEmptyColor          => lbEmptyColor;
        public Color ActiveButtonColor     => activeButtonColor;
        public Color TileColor             => tileColor;
        public Color TotalTilesCounterMultiplyThreeColor => totalTilesCounterMultiplyThreeColor;
        public Color TransparentTileColor  => transparentTileColor;
        public AssetLabelReference DataLevelsReference => dataLevelsReference;

        public Color GetPreviewLayerColor(int id)
        {
            return id switch
            {
                0 => previewLayer_1_Color,
                1 => previewLayer_2_Color,
                2 => previewLayer_3_Color,
                3 => previewLayer_4_Color,
                _ => previewLayer_5_Color,
            };
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (gridLayoutGroupCells == null)
            {
                gridLayoutGroupCells = FindObjectOfType<FlexibleGridLayout>();

                if (gridLayoutGroupCells == null)
                    return;
            }

            gridLayoutGroupCells.rows    = GridSize.y;
            gridLayoutGroupCells.spacing = Spacing;
            gridLayoutGroupCells.CalculateLayoutInputHorizontal();

            Version = VERSION;
        }
#endif
    }
}
