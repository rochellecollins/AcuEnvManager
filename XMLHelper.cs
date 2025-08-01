using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace AcuEnvManager
{
    public static class XMLHelper
    {
        /// <summary>
        /// Saves a list of WorktreeModel objects to an XML file.
        /// </summary>
        public static void SaveWorktrees(string filePath, IEnumerable<WorktreeModel> worktrees)
        {
            // Ensure the directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var doc = new XDocument(
                new XElement("Worktrees",
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
                )
            );
            doc.Save(filePath);
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

        /// <summary>
        /// Loads application settings (as key-value pairs) from an XML file.
        /// </summary>
        public static Dictionary<string, string> LoadSettings(string filePath)
        {
            var result = new Dictionary<string, string>();
            if (!File.Exists(filePath))
                return result;

            var doc = XDocument.Load(filePath);
            foreach (var settingElem in doc.Descendants("Setting"))
            {
                var key = settingElem.Attribute("Key")?.Value;
                var value = settingElem.Attribute("Value")?.Value;
                if (key != null)
                    result[key] = value ?? "";
            }
            return result;
        }

        internal static WorktreeModel SaveWorktree(string settingsPath, WorktreeModel wt)
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

            SaveWorktrees(settingsPath, worktrees);
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

            var doc = new XDocument(
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
            );
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
    }
}