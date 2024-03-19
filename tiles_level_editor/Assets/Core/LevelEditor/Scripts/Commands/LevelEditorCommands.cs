public class LevelEditorCommands
{
    public struct Save : ICommandModel
    {
        public string Name;
        public bool IsOverwrite;
        public bool OverwriteValue;
    }

    public struct Load : ICommandModel
    {
    }

    public struct Switch : ICommandModel 
    {
        public int ID;
    }
    public struct Preview : ICommandModel { }
    public struct Copy : ICommandModel { }
    public struct Clear : ICommandModel { }
    public struct Transparent : ICommandModel { }
}