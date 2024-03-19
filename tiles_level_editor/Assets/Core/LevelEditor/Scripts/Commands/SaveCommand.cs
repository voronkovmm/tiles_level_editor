using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using static LevelEditorCommands;

namespace Core.Scripts.LevelEditor
{
    public class SaveCommand : AbstractLevelEditorCommand<Save>
    {
        private string name;
        private LevelDataSO existingData;

        public SaveCommand(LevelEditorPresenter presenter) : base(presenter) { }

        public override void Execute(LevelEditorCommands.Save save)
        {
#if UNITY_EDITOR
            if (save.IsOverwrite)
                OnClickOverwrite(save.OverwriteValue);

            if (!model.IsValidateActive)
            {
                model.SetValidateActive(true);
                return;
            }

            name = save.Name;

            if (string.IsNullOrEmpty(name))
            {
                presenter.InvokePopupOverwriteSetActive(false);
                model.SetValidateActive(false);
                return;
            }

            bool fileExist = FileExist(model.PATH_TO_SAVE, name, out string pathToFile);
            if (fileExist)
            {
                existingData = AssetDatabase.LoadAssetAtPath<LevelDataSO>(pathToFile);
                presenter.InvokePopupOverwriteSetActive(true);
                return;
            }
            else
                Save();
        }

        private bool FileExist(string path, string name, out string pathToFile)
        {
            string[] files = Directory.GetFiles(path);
            pathToFile = files.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).Equals(name, StringComparison.OrdinalIgnoreCase));
            return File.Exists(pathToFile);
        }

        private void Save()
        {
            model.SetValidateActive(false);
            model.SetPreviewActive(false);

            List<NewGrid<NewCell>> layers = model.GetLayers();

            string fileName = model.GetFileNameWithExtension(name);
            string path = Path.Combine(model.PATH_TO_SAVE, fileName);

            presenter.ViewClickPreview();

            if (existingData != null)
            {
                existingData.SetNewData(model.LevelVersion, layers);
                existingData = null;
            }
            else
                LevelDataSO.CreateScriptableObject(path, layers, model.ModelSO.Version, fileName);

            presenter.ViewClickPreview();
        }

        public void OnClickOverwrite(bool value)
        {
            presenter.InvokePopupOverwriteSetActive(false);

            if (value)
                Save();

            existingData = null;
            view.ClearTemplateName();
        }
#endif
    }
}
