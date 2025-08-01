public class SettingsModel
{
    public string RepoPath { get; }
    public string SettingsPath { get; set; }
    public string BuildsPath { get; }
    public string LocalBuildPath { get; }
    public DateTime LastSave { get; set; }
    public bool FastCopy { get; set; }

    public SettingsModel()
    {
        RepoPath = @"C:\Users\rochelle.collins\source\repos";
        BuildsPath = @"\\int\DFS\Builds\Builds";
        LocalBuildPath = @"D:\AcumaticaInstalls";
    }
}
