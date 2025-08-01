namespace AcuEnvManager
{
    public class ActionController
    {
        public SettingsModel settings { get; }

        public ActionController(SettingsModel settings)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        internal async Task<bool> ExecuteActionAsync(ActionModel action, IProgress<string> progress)
        {
            bool result = true;
            switch (action.ActionName)
            {
                case ActionType.Checkout:
                    progress.Report("Checking out branch...");
                    result = await CommandHelper.CheckOutBranchAsync(Path.Combine(settings.RepoPath, action.WorkTreeName), action.Value, progress);
                    progress.Report("Checkout complete.");
                    break;
                case ActionType.Acubuild:
                    progress.Report("Running Acubuild...");
                    result = await CommandHelper.AcubuildAsync(Path.Combine(settings.RepoPath, action.WorkTreeName), progress);
                    progress.Report("Acubuild complete.");
                    break;
                case ActionType.Update:                   
                    progress.Report("Updating...");
                    switch(action.Entity)
                    {
                        case EntityType.WebConfig:
                            result = await FileSystemHelper.UpdateWebConfigAsync(Path.Combine(settings.RepoPath, action.WorkTreeName), action.Value, progress);
                            break;
                    }
                    progress.Report("Update complete.");
                    break;
                case ActionType.Download:
                    progress.Report("Downloading...");
                    result = await FileSystemHelper.DownloadFileAsync(settings, action.Value, progress);
                    progress.Report("Download complete.");
                    break;
            }
            return result;

        }
    }
}