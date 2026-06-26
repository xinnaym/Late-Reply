using Godot;

public partial class SaveSystem : Node
{
    public static SaveSystem Instance { get; private set; }

    private const string SAVE_PATH = "user://savegame.json";

    public override void _Ready()
    {
        Instance = this;
    }

    public void SaveGame()
    {
        var data = new Godot.Collections.Dictionary
        {
            { "scene", GetTree().CurrentScene?.SceneFilePath },
            { "timestamp", Time.GetDatetimeStringFromSystem() }
        };

        var file = FileAccess.Open(SAVE_PATH, FileAccess.ModeFlags.Write);
        if (file != null)
        {
            file.StoreString(Json.Stringify(data));
            file.Close();
        }
    }

    public bool HasSave()
    {
        return FileAccess.FileExists(SAVE_PATH);
    }

    public void DeleteSave()
    {
        if (FileAccess.FileExists(SAVE_PATH))
        {
            DirAccess.RemoveAbsolute(SAVE_PATH);
        }
    }
}
