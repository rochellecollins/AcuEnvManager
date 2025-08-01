namespace AcuEnvManager
{
    public class ActionController
    {
        public string repoPath { get; set; }

        public ActionController(string path)
        {
            repoPath = path;
        }

        internal async Task<bool> ExecuteActionAsync(ActionModel action, IProgress<string> progress)
        {
            bool result = true;
            switch (action.ActionName)
            {
                case ActionType.Checkout:
                    progress.Report("Checking out branch...");
                    result = await CommandHelper.CheckOutBranchAsync(Path.Combine(repoPath, action.EntityName), action.Value, progress);
                    progress.Report("Checkout complete.");
                    break;
                case ActionType.Acubuild:
                    progress.Report("Running Acubuild...");
                    result = await CommandHelper.AcubuildAsync(Path.Combine(repoPath, action.EntityName), progress);
                    progress.Report("Acubuild complete.");
                    break;
                case ActionType.Update:                   
                    progress.Report("Updating...");
                    switch(action.Entity)
                    {
                        case EntityType.WebConfig:
                            result = await FileSystemHelper.UpdateWebConfigAsync(Path.Combine(repoPath, action.EntityName), action.Value, progress);
                            break;
                    }
                    progress.Report("Update complete.");
                    break;
            }
            return result;

        }
    }
}