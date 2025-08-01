using System.ComponentModel;

namespace AcuEnvManager
{
    public class WorktreeController
    {
        public string repoPath { get; set; }
        public string settingsPath { get; set; }

        public DatabaseHelper? dbHelper { get; set; }

        public WorktreeController(string path, string settingsPath)
        {
            repoPath = path;
            this.settingsPath = settingsPath;
        }

        internal BindingList<WorktreeModel> GetWorktrees()
        {
            var repos = FileSystemHelper.GetDirectories(repoPath);

            var worktreeList = new BindingList<WorktreeModel>();
            var savedWorktrees = XMLHelper.LoadWorktrees(settingsPath);

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

        public bool UpdateWorktreeDetails(WorktreeModel wt, IProgress<string> progress)
        {
            bool changed = false;
            var newCodeVers = CommandHelper.GetLatestTagAsync(Path.Combine(repoPath, wt.WorkTreeName), progress).Result ?? string.Empty;
            if(newCodeVers != wt.CodeVers)
            {
                wt.CodeVers = newCodeVers;
                changed = true;
            }
            var newBranchName = CommandHelper.GetCurrentBranchAsync(Path.Combine(repoPath, wt.WorkTreeName), progress).Result ?? string.Empty;
            if(newBranchName != wt.BranchName)
            {
                wt.BranchName = newBranchName;
                wt.JIRALink = GetJIRALink(wt.BranchName);
                changed = true;
            }
            var serverAndDatabase = FileSystemHelper.GetServerAndDatabaseFromWebConfig(Path.Combine(repoPath, wt.WorkTreeName));
            if (wt.DBServer != serverAndDatabase.ServerName || wt.DBName != serverAndDatabase.DatabaseName)
            {
                wt.DBServer = serverAndDatabase.ServerName ?? "None";
                wt.DBName = serverAndDatabase.DatabaseName ?? "None";
                changed = true;
            }
            if (wt.DBServer != "None" && wt.DBName != "None")
            {
                var newDBVers = new DatabaseHelper(wt.DBServer, wt.DBName).GetDatabaseVersion() ?? string.Empty;
                if (newDBVers != wt.DBVers)
                {
                    wt.DBVers = newDBVers;
                    changed = true;
                }
            }

            // Save the updated worktree details to XML
            if (changed)
                XMLHelper.SaveWorktree(settingsPath, wt);

            return changed;
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