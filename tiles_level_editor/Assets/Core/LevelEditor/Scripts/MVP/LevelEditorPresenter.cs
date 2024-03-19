using Spine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using static LevelEditorCommands;

namespace Core.Scripts.LevelEditor
{
    public class LevelEditorPresenter
    {
        public ViewLevelEditor View;
        public LevelEditorModel Model;
        public TileFactory TileFactory;

        public event Action<bool> OnPopupOverwriteSetActive;
        public event Action<bool> OnAnticlickSetActive;

        private GostShower gostShower;
        private LevelEditorInputHandler inputHandler;
        private CommandExecutor<ICommandModel> commandExecutor;

        public LevelEditorPresenter(ViewLevelEditor view, LevelEditorModel model, RectTransform gridRect, RectTransform parentTiles)
        {
            this.View = view;
            this.Model = model;

            TileFactory = new TileFactory(model.ModelSO, model.Grid.CellSize, parentTiles);

            Model.OnPreviewStateChange += LockInput;

            inputHandler = new LevelEditorInputHandler(this);
            inputHandler.AddTile += HandlerAddTile;
            inputHandler.RemoveTile += HandlerRemoveTile;

            gostShower = new GostShower(this);

            commandExecutor = new();
            commandExecutor.AddCommand(new SaveCommand(this));
            commandExecutor.AddCommand(new LoadCommand(this));
            commandExecutor.AddCommand(new CopyBottomLayerCommand(this));
            commandExecutor.AddCommand(new ClearCommand(this));
            commandExecutor.AddCommand(new SwitchLayerCommand(this));
            commandExecutor.AddCommand(new PreviewCommand(this));
            commandExecutor.AddCommand(new TransparentLayer(this));

            ViewDisplayVersion();
        }

        public void Execute<T>(T arg) where T : struct, ICommandModel => commandExecutor.Execute(in arg);

        public void ViewClickCopy() => Execute(new Copy());
        public void ViewClickLoad() => Execute(new Load());
        public void ViewClickSave() => Execute(new Save());
        public void ViewClickClear() => Execute(new Clear());
        public void ViewClickPreview() => Execute(new Preview());
        public void ViewTransparentLayer() => Execute(new Transparent());
        public void ViewClickLayer(int id) => Execute(new Switch() { ID = id });
        public void ViewOnClickOverwrite(bool value) => Execute(new Save() { IsOverwrite = true, OverwriteValue = value });
        public void ViewClickValidateSave(string name) => Execute(new Save() { Name = name });
        public void ViewDisplayVersion() => View.DisplayVersion(Model.ModelSO.Version, Model.LevelVersion);
        
        public void InvokePopupOverwriteSetActive(bool value)
        {
            LockInput(value);
            InvokeAnticlickSetActive(value);
            OnPopupOverwriteSetActive?.Invoke(value);
        }
        public void InvokeAnticlickSetActive(bool value) => OnAnticlickSetActive?.Invoke(value);
        public void LockInput(bool value) => InputManager.LockInput = value;
        public void HandlerOnInputSelect() => LockInput(true);
        public void HandlerOnInputDeselect() => LockInput(false);
        public void ShowGost() => gostShower.Show();

        private void HandlerRemoveTile() => Model.RemoveTile();
        private void HandlerAddTile() => Model.AddTile();
    } 
}

