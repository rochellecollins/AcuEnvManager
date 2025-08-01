using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Linq;

namespace AcuEnvManager
{
    public static class XMLHelper
    {
        /// <summary>
        /// Saves a list of WorktreeModel objects to an XML file.
        /// Ensures that existing Builds are not overwritten.
        /// </summary>
        public static void SaveWorktrees(string filePath, IEnumerable<WorktreeModel> worktrees, IProgress<string> progress)
        {
            // Ensure the directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            XDocument doc;
            XElement? buildsElem = null;

            if (File.Exists(filePath))
            {
                // Load existing XML and preserve Builds node
                doc = XDocument.Load(filePath);
                buildsElem = doc.Root?.Element("Builds");
                // Remove existing Worktrees node if present
                var worktreesElem = doc.Root?.Element("Worktrees");
                worktreesElem?.Remove();
            }
            else
            {
                // Create new XML root
                doc = new XDocument(new XElement("Root"));
            }

            var newWorktreesElem = new XElement("Worktrees",
                new XElement("SavedAt", DateTime.UtcNow),
                new XElement("Items",
                    worktrees != null
                        ? new List<XElement>(
                            worktrees.Select(wt =>
                                new XElement("Worktree",
                                    new XElement("WorkTreeName", wt.WorkTreeName),
                                    new XElement("CodeVers", wt.CodeVers),
                                    new XElement("BranchName", wt.BranchName),
                                    new XElement("DBServer", wt.DBServer),
                                    new XElement("DBName", wt.DBName),
                                    new XElement("DBVers", wt.DBVers),
                                    new XElement("JIRALink", wt.JIRALink)
                                )
                            ))
                        : null
                )
            );

            doc.Root?.Add(newWorktreesElem);

            // Re-add Builds if it existed
            if (buildsElem != null)
            {
                doc.Root?.Add(buildsElem);
            }

            // Check if file is locked by another process
            if (File.Exists(filePath))
            {
                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        doc.Save(filePath);
                    }
                }
                catch (IOException)
                {
                    progress.Report($"The file '{filePath}' is currently in use by another process.");
                }
            }

        }

        /// <summary>
        /// Loads a list of WorktreeModel objects from an XML file.
        /// </summary>
        public static List<WorktreeModel> LoadWorktrees(string filePath)
        {
            var result = new List<WorktreeModel>();
            if (!File.Exists(filePath))
                return result;

            try
            {
                var doc = XDocument.Load(filePath);
                foreach (var wtElem in doc.Descendants("Worktree"))
                {
                    result.Add(new WorktreeModel
                    {
                        WorkTreeName = wtElem.Element("WorkTreeName")?.Value ?? "",
                        CodeVers = wtElem.Element("CodeVers")?.Value ?? "",
                        BranchName = wtElem.Element("BranchName")?.Value ?? "",
                        DBServer = wtElem.Element("DBServer")?.Value ?? "",
                        DBName = wtElem.Element("DBName")?.Value ?? "",
                        DBVers = wtElem.Element("DBVers")?.Value ?? "",
                        JIRALink = wtElem.Element("JIRALink")?.Value ?? ""
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading worktrees from XML: {ex.Message}");
                return result;
            }
        }
        /// <summary>
        /// Saves application settings (as key-value pairs) to an XML file.
        /// </summary>
        public static void SaveSettings(string filePath, Dictionary<string, string> settings)
        {
            var doc = new XDocument(
                new XElement("Settings",
                    new XElement("SavedAt", DateTime.UtcNow),
                    new XElement("Items",
                        settings != null
                            ? new List<XElement>(
                                settings.Select(kvp =>
                                    new XElement("Setting",
                                        new XAttribute("Key", kvp.Key),
                                        new XAttribute("Value", kvp.Value)
                                    )
                                ))
                            : null
                    )
                )
            );
            doc.Save(filePath);
        }



        internal static WorktreeModel SaveWorktree(string settingsPath, WorktreeModel wt, IProgress<string> progress)
        {
            if (wt == null)
                throw new ArgumentNullException(nameof(wt));

            List<WorktreeModel> worktrees = new List<WorktreeModel>();
            if (File.Exists(settingsPath))
            {
                worktrees = LoadWorktrees(settingsPath);
            }

            // Replace if exists (by WorkTreeName), else add
            var existing = worktrees.FindIndex(w => w.WorkTreeName == wt.WorkTreeName);
            if (existing >= 0)
                worktrees[existing] = wt;
            else
                worktrees.Add(wt);

            SaveWorktrees(settingsPath, worktrees, progress);
            return wt;
        }

        internal static void SaveBuilds(string settingsPath, List<BuildModel> buildList)
        {
            // Ensure the directory exists
            var directory = Path.GetDirectoryName(settingsPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            XDocument doc;
            if (File.Exists(settingsPath))
            {
                // Load existing XML and preserve Worktrees
                doc = XDocument.Load(settingsPath);

                // Remove existing Builds node if present
                var buildsElem = doc.Root?.Element("Builds");
                buildsElem?.Remove();

                // Add new Builds node
                var newBuildsElem = new XElement("Builds",
                    new XElement("SavedAt", DateTime.UtcNow),
                    new XElement("Items",
                        buildList != null
                            ? new List<XElement>(
                                buildList.Select(b =>
                                    new XElement("Build",
                                        new XElement("MajorVersion", b.MajorVersion ?? ""),
                                        new XElement("MinorVersions",
                                            b.MinorVersions != null
                                                ? new List<XElement>(
                                                    b.MinorVersions.Select(mv =>
                                                        new XElement("MinorVersion", mv ?? "")
                                                    ))
                                                : null
                                        )
                                    )
                                ))
                            : null
                    )
                );

                doc.Root?.Add(newBuildsElem);
            }
            else
            {
                // Create new XML with only Builds
                doc = new XDocument(
                    new XElement("Root",
                        new XElement("Builds",
                            new XElement("SavedAt", DateTime.UtcNow),
                            new XElement("Items",
                                buildList != null
                                    ? new List<XElement>(
                                        buildList.Select(b =>
                                            new XElement("Build",
                                                new XElement("MajorVersion", b.MajorVersion ?? ""),
                                                new XElement("MinorVersions",
                                                    b.MinorVersions != null
                                                        ? new List<XElement>(
                                                            b.MinorVersions.Select(mv =>
                                                                new XElement("MinorVersion", mv ?? "")
                                                            ))
                                                        : null
                                                )
                                            )
                                        ))
                                    : null
                            )
                        )
                    )
                );
            }
            doc.Save(settingsPath);
        }

        internal static List<BuildModel> LoadBuilds(string settingspath)
        {
            var result = new List<BuildModel>();
            if (!File.Exists(settingspath))
                return result;

            try
            {
                var doc = XDocument.Load(settingspath);
                foreach (var buildElem in doc.Descendants("Build"))
                {
                    var majorVersion = buildElem.Element("MajorVersion")?.Value ?? "";
                    var minorVersions = new List<string>();
                    var minorVersionsElem = buildElem.Element("MinorVersions");
                    if (minorVersionsElem != null)
                    {
                        foreach (var mvElem in minorVersionsElem.Elements("MinorVersion"))
                        {
                            minorVersions.Add(mvElem.Value ?? "");
                        }
                    }
                    result.Add(new BuildModel
                    {
                        MajorVersion = majorVersion,
                        MinorVersions = minorVersions
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading builds from XML: {ex.Message}");
                return result;
            }
        }

        internal static void SaveData(BindingList<WorktreeModel> worktreeList, List<BuildModel> buildList, SettingsModel settings, IProgress<string> getLogger)
        {
            if (settings == null || string.IsNullOrWhiteSpace(settings.SettingsPath))
                throw new ArgumentNullException(nameof(settings));

            // Ensure the directory exists
            var directory = Path.GetDirectoryName(settings.SettingsPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            settings.LastSave = DateTime.Now;

            // Prepare Worktrees XML
            var worktreesElem = new XElement("Worktrees",
                new XElement("SavedAt", DateTime.UtcNow),
                new XElement("Items",
                    worktreeList != null
                        ? new List<XElement>(
                            worktreeList.Select(wt =>
                                new XElement("Worktree",
                                    new XElement("WorkTreeName", wt.WorkTreeName),
                                    new XElement("CodeVers", wt.CodeVers),
                                    new XElement("BranchName", wt.BranchName),
                                    new XElement("DBServer", wt.DBServer),
                                    new XElement("DBName", wt.DBName),
                                    new XElement("DBVers", wt.DBVers),
                                    new XElement("JIRALink", wt.JIRALink)
                                )
                            ))
                        : null
                )
            );

            // Prepare Builds XML
            var buildsElem = new XElement("Builds",
                new XElement("SavedAt", DateTime.UtcNow),
                new XElement("Items",
                    buildList != null
                        ? new List<XElement>(
                            buildList.Select(b =>
                                new XElement("Build",
                                    new XElement("MajorVersion", b.MajorVersion ?? ""),
                                    new XElement("MinorVersions",
                                        b.MinorVersions != null
                                            ? new List<XElement>(
                                                b.MinorVersions.Select(mv =>
                                                    new XElement("MinorVersion", mv ?? "")
                                                ))
                                            : null
                                    )
                                )
                            ))
                        : null
                )
            );

            // Prepare Settings XML (all public string properties without reflection)
            var settingsDict = new Dictionary<string, string>
            {
                { nameof(SettingsModel.RepoPath), settings.RepoPath ?? "" },
                { nameof(SettingsModel.SettingsPath), settings.SettingsPath ?? "" },
                { nameof(SettingsModel.BuildsPath), settings.BuildsPath ?? "" },
                { nameof(SettingsModel.LocalBuildPath), settings.LocalBuildPath ?? "" },
                {nameof(SettingsModel.LastSave),settings.LastSave.ToString("o") }
            };

            var settingsElem = new XElement("Settings",
                new XElement("SavedAt", DateTime.UtcNow),
                new XElement("Items",
                    settingsDict.Select(kvp =>
                        new XElement("Setting",
                            new XAttribute("Key", kvp.Key),
                            new XAttribute("Value", kvp.Value)
                        )
                    )
                )
            );

            // Compose root XML
            var doc = new XDocument(
                new XElement("Root",
                    worktreesElem,
                    buildsElem,
                    settingsElem
                )
            );

            try
            {
                doc.Save(settings.SettingsPath);
                getLogger?.Report($"Data saved to '{settings.SettingsPath}'.");
            }
            catch (Exception ex)
            {
                getLogger?.Report($"Error saving data: {ex.Message}");
            }
        }

        internal static SettingsModel LoadSettings(string settingsPath)
        {
            if (string.IsNullOrWhiteSpace(settingsPath) || !File.Exists(settingsPath))
                return new SettingsModel();

            try
            {
                var doc = XDocument.Load(settingsPath);
                var settingsElem = doc.Descendants("Settings").FirstOrDefault();
                if (settingsElem == null)
                    return new SettingsModel();

                var itemsElem = settingsElem.Element("Items");
                if (itemsElem == null)
                    return new SettingsModel();

                var settingsDict = itemsElem.Elements("Setting")
                    .Where(e => e.Attribute("Key") != null)
                    .ToDictionary(
                        e => e.Attribute("Key")!.Value,
                        e => e.Attribute("Value")?.Value ?? ""
                    );

                var model = new SettingsModel();

                // Set string properties
                var type = typeof(SettingsModel);
                foreach (var kvp in settingsDict)
                {
                    var prop = type.GetProperty(kvp.Key, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (prop != null && prop.CanWrite)
                    {
                        if (prop.PropertyType == typeof(string))
                        {
                            prop.SetValue(model, kvp.Value);
                        }
                        else if (prop.PropertyType == typeof(DateTime) && kvp.Key == nameof(SettingsModel.LastSave))
                        {
                            if (DateTime.TryParse(kvp.Value, null, System.Globalization.DateTimeStyles.RoundtripKind, out var dt))
                                prop.SetValue(model, dt);
                        }
                    }
                }
                model.SettingsPath = settingsPath;
                return model;
            }
            catch
            {
                return new SettingsModel();
            }
        }
    }
}