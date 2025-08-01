namespace AcuEnvManager
{
    public class ActionModel
    {
        public string WorkTreeName { get; set; }    
        public string ActionName { get; internal set; }
        public string Entity { get; internal set; }
        public string EntityName { get; internal set; }
        public string Value { get; internal set; }
        public string Status { get; set; }

        public string ActionDescription
        {
            get
            {
                return ActionName switch
                {
                    ActionType.Delete => "Delete",
                    ActionType.Update => "Update",
                    ActionType.Checkout => "Checkout",
                    ActionType.Acubuild => "Acubuild",
                    ActionType.Download => "Download",
                    _ => ActionName
                };
            }
        }

        public string EntityDescription
        {
            get
            {
                return Entity switch
                {
                    EntityType.Worktree => "Worktree",
                    EntityType.WebConfig => "Web Config",
                    EntityType.Database => "Database",
                    EntityType.Build => "Build",
                    _ => EntityName
                };
            }
        }
    }

    public class EntityType
    {
        public const string Worktree = "W";
        public const string WebConfig = "C";
        public const string Database = "D";
        public const string Build = "B";
    }

    public class ActionType
    {
        public const string Delete = "D";
        public const string Update = "U";
        public const string Checkout = "C";
        public const string Acubuild = "A";
        public const string Download = "DL";
        // Add more action types as needed
    }
}