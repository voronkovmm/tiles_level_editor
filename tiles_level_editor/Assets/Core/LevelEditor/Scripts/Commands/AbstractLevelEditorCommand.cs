using Core.Scripts.LevelEditor;
using System;

public abstract class AbstractLevelEditorCommand<T> where T : ICommandModel
{
    protected readonly ViewLevelEditor view;
    protected readonly LevelEditorModel model;
    protected readonly LevelEditorPresenter presenter;

    public AbstractLevelEditorCommand(LevelEditorPresenter presenter)
    {
        this.presenter = presenter;
        this.model = presenter.Model;
        this.view = presenter.View;
    }

    public abstract void Execute(T arg);
}
