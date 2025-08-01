using System.ComponentModel;

namespace AcuEnvManager
{
    public class WorktreeController
    {

        public DatabaseHelper? dbHelper { get; set; }
        public SettingsModel settings { get; set; }

        public WorktreeController(SettingsModel settings)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        internal BindingList<WorktreeModel> GetWorktrees()
        {
            var repos = FileSystemHelper.GetDirectories(settings.RepoPath);

            var worktreeList = new BindingList<WorktreeModel>();
            var savedWorktrees = XMLHelper.LoadWorktrees(settings.SettingsPath);

            foreach (var repo in repos)
            {
                if (CommandHelper.IsGitRepository(repo))
                {   
                    var repoName = Path.GetFileName(repo);
                    var savedWorktree = savedWorktrees.FirstOrDefault(wt => wt.WorkTreeName == repoName);
                    var worktree = new WorktreeModel
                    {
                        WorkTreeName = repoName,
                        CodeVers = savedWorktree?.CodeVers ?? "Loading...",
                        BranchName = savedWorktree?.BranchName ?? "Loading...",
                        JIRALink = savedWorktree?.JIRALink ?? "Loading...",
                        DBServer = savedWorktree?.DBServer ?? "Loading...",
                        DBName = savedWorktree?.DBName ?? "Loading...",
                        DBVers = savedWorktree?.DBVers ?? "Loading..."
                    };

                    worktreeList.Add(worktree);
                }
            }

            return worktreeList;
        }

        public void UpdateWorktreeDetails(WorktreeModel wt, IProgress<string> progress)
        {
            wt.CodeVers = CommandHelper.GetLatestTagAsync(Path.Combine(settings.RepoPath, wt.WorkTreeName), progress).Result ?? string.Empty;

            wt.BranchName = CommandHelper.GetCurrentBranchAsync(Path.Combine(settings.RepoPath, wt.WorkTreeName), progress).Result ?? string.Empty;
            wt.JIRALink = GetJIRALink(wt.BranchName);

            var serverAndDatabase = FileSystemHelper.GetServerAndDatabaseFromWebConfig(Path.Combine(settings.RepoPath, wt.WorkTreeName));
            wt.DBServer = serverAndDatabase.ServerName ?? "None";
            wt.DBName = serverAndDatabase.DatabaseName ?? "None";

            if (wt.DBServer != "None" && wt.DBName != "None")
            {
                wt.DBVers = new DatabaseHelper(wt.DBServer, wt.DBName).GetDatabaseVersion() ?? string.Empty;
            }
            else
            {
                wt.DBVers = "N/A";
            }
        }

        private string GetJIRALink(string branchName)
        {
            // Use regex to extract "AC-" followed by digits
            var match = System.Text.RegularExpressions.Regex.Match(branchName, @"AC-\d+");
            if (match.Success)
            {
                var jiranumber = match.Value;
                return $"https://jira.acumatica.com/browse/{jiranumber}";
            }
            return string.Empty;
        }
    }
}