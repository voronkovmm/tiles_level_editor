using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

namespace Core.Scripts.LevelEditor
{
    public class ViewLevelEditor : MonoBehaviour
    {
        private LevelEditorModel model;
        private LevelEditorPresenter presenter;
        private Tween tweenPreviewBtnScale;

        [Header("ModelSO")]
        [SerializeField] private LevelEditorModelSO modelSO;

        [Header("LayerButtons")]
        [SerializeField] private List<LayerButton> layerButtons;

        [Header("PopupOverwrite")]
        [SerializeField] private GameObject popupOverwrite;

        [Header("Components")]
        [SerializeField] private RectTransform gridRect;
        [SerializeField] private RectTransform parentTiles;
        [SerializeField] private FlexibleGridLayout gridLayoutGroupCells;
        [SerializeField] private Image imgBtnPreview;
        [SerializeField] private GameObject btnValidateSave;
        [SerializeField] private GameObject anticlick;
        [SerializeField] private ButtonWithText btnSave;
        [SerializeField] private TMP_Text tmpBtnPreview;
        [SerializeField] private TMP_Text tmpEditorVersion;
        [SerializeField] private TMP_Text tmpLevelVersion;
        [SerializeField] private TMP_Text tmpTotalFigures;
        [SerializeField] private TMP_Text tmpTotalFiguresOnLayer;
        [SerializeField] private TMP_InputField tmpInputTemplateName;

        public enum EnumLayerColor { ACTIVE, NOT_EMPTY, EMPTY }

        public bool IsValidateBtnActiveSelf => btnValidateSave.activeSelf;

        private void Awake()
        {
            gridLayoutGroupCells.rows    = modelSO.GridSize.y;
            gridLayoutGroupCells.spacing = modelSO.Spacing;
            gridLayoutGroupCells.CalculateLayoutInputHorizontal();

            model     = new(this, presenter, modelSO, gridRect, gridLayoutGroupCells.cellSize);
            presenter = new(this, model, gridRect, parentTiles);

            HandlerValidateStateChange(false);
            HandlerPopupOverwriteSetActive(false);
            SetLayerColor(0, EnumLayerColor.ACTIVE);

            model.OnPreviewStateChange += PreviewBtnSetActive;
            model.OnValidateStateChange += HandlerValidateStateChange;
            presenter.OnAnticlickSetActive += HandlerAnticlickSetActive;
            presenter.OnPopupOverwriteSetActive += HandlerPopupOverwriteSetActive;
            tmpInputTemplateName.onSelect.AddListener(HandlerOnInputSelect);
            tmpInputTemplateName.onDeselect.AddListener(HandlerOnInputDeselect);
        }

        private void Update() => presenter.ShowGost();

        public void OnClickLayer(int num) => presenter.ViewClickLayer(num);
        public void OnClickCopy()         => presenter.ViewClickCopy();
        public void OnClickPreview()      => presenter.ViewClickPreview();
        public void OnClickRun()          => Debug.Log("run");
        public void OnClickSave()         => presenter.ViewClickSave();
        public void OnClickValidateSave() => presenter.ViewClickValidateSave(tmpInputTemplateName.text);
        public void OnClickLoad()         => presenter.ViewClickLoad();
        public void OnClickClear()        => presenter.ViewClickClear();
        public void OnClickOverwrite(bool value) => presenter.ViewOnClickOverwrite(value);

        public void SetLayerColor(int id, EnumLayerColor colorType)
        {
            if (id < 0 || id > layerButtons.Count - 1)
                return;

            layerButtons[id].Color = colorType switch
            {
                EnumLayerColor.ACTIVE     => modelSO.ActiveButtonColor,
                EnumLayerColor.NOT_EMPTY  => modelSO.LbNotEmptyColor,
                _ => modelSO.LbEmptyColor
            };
        }
        public void DisplayVersion(string editorVersion, string levelVersion)
        {
            tmpEditorVersion.text = $"Версия редактора: {editorVersion}";
            tmpLevelVersion.text = $"Версия уровня: {levelVersion}";

            Color levelVersionColor = editorVersion.Equals(levelVersion) ? modelSO.FontColor : Color.red;
            tmpLevelVersion.color = levelVersionColor;
        }
        public void ClearTemplateName() => tmpInputTemplateName.text = "";
        public void TweenScalePreviewBtn()
        {
            Transform tr = imgBtnPreview.transform;

            if (tweenPreviewBtnScale != null && tweenPreviewBtnScale.active)
                tweenPreviewBtnScale.Kill();

            tweenPreviewBtnScale = tr.DOScale(new Vector2(1.07f, 1.07f), .08f)
              .SetLoops(1, LoopType.Yoyo)
              .OnKill(() => tr.localScale = Vector3.one);
        }
        public void RefreshTileCounters(int layerTiles, int totalTiles)
        {
            var totalTilesHexColor = ColorUtility.ToHtmlStringRGBA((totalTiles > 0 && totalTiles % 3 == 0 ? modelSO.TotalTilesCounterMultiplyThreeColor : modelSO.FontColor));
            tmpTotalFigures.text = $"Всего фигур: <color=#{totalTilesHexColor}>{totalTiles}";
            tmpTotalFiguresOnLayer.text = $"Фигур на слое {layerTiles}";
        }

        private void HandlerPopupOverwriteSetActive(bool value)
        {
            popupOverwrite.SetActive(value);
            anticlick.SetActive(value);
        }
        private void HandlerOnInputSelect(string arg) => presenter.HandlerOnInputSelect();
        private void HandlerOnInputDeselect(string arg) => presenter.HandlerOnInputDeselect();
        private void HandlerValidateStateChange(bool value)
        {
            btnSave.Text  = value ? "Отменить" : "Сохранить";
            btnSave.Color = value ? modelSO.ActiveButtonColor : Color.white;
            btnValidateSave.SetActive(value);
        }
        private void HandlerAnticlickSetActive(bool value) => anticlick.SetActive(value);
        private void PreviewBtnSetActive(bool value)
        {
            imgBtnPreview.color = value ? modelSO.ActiveButtonColor : Color.white;
            tmpBtnPreview.text  = value ? "Убрать превью" : "Показать превью";
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!modelSO.DebugEnable)
                return;

            Gizmos.color = Color.red;
            DrawGrid();
            Gizmos.color = Color.white;
        }

        private void DrawGrid()
        {
            Vector2 cellSize = gridLayoutGroupCells.cellSize + modelSO.Spacing;

            Vector3 worldCellSize = transform.TransformPoint(new Vector2(cellSize.x, cellSize.y));
            Vector3 offset = worldCellSize * .5f;

            Vector2 anchoreCenterPos = Voron.Voron.TransformToAnchor(gridRect.anchoredPosition, new Vector2(gridRect.anchorMin.x, gridRect.anchorMin.y), (RectTransform)gridRect.parent, Voron.Voron.EnumAnchor.CENTER);

            for (int x = 0; x < modelSO.GridSize.x; x++)
                for (int y = 0; y < modelSO.GridSize.y; y++)
                {
                    Vector2 index = new Vector2(x, y);
                    Vector2 rectCoordinate = anchoreCenterPos + (cellSize * index) - modelSO.Spacing / 2;

                    Vector2 pos = transform.TransformPoint(rectCoordinate) + offset;

                    Gizmos.DrawWireCube(pos, worldCellSize);
                }
        }
#endif
    }
}
