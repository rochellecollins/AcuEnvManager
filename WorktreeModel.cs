using System;

namespace AcuEnvManager
{
    public class WorktreeModel
    {
        public string WorkTreeName { get; set; }
        public string CodeVers { get; set; }
        public string DBServer { get; set; }
        public string DBName { get; set; }
        public string DBVers { get; set; }
        public string JIRALink { get; set; }
        public string BranchName { get; set; }
    }
}
