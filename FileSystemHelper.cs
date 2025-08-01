using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace AcuEnvManager
{
    public static class FileSystemHelper
    {
        /// <summary>
        /// Gets all file names in the specified directory.
        /// </summary>
        public static IEnumerable<string> GetFiles(string directoryPath, string searchPattern = "*.*", bool recursive = false)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("Directory path cannot be null or empty.", nameof(directoryPath));

            var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            return Directory.Exists(directoryPath)
                ? Directory.GetFiles(directoryPath, searchPattern, option)
                : Array.Empty<string>();
        }

        /// <summary>
        /// Gets all folder names in the specified directory.
        /// </summary>
        public static IEnumerable<string> GetDirectories(string directoryPath, bool recursive = false)
        {
            var stopwatch = Stopwatch.StartNew();

            if (string.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("Directory path cannot be null or empty.", nameof(directoryPath));

            var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var list = Directory.Exists(directoryPath)
                ? Directory.GetDirectories(directoryPath, "*", option)
                : Array.Empty<string>();

            stopwatch.Stop();
            Debug.WriteLine($"GetDirectories took {stopwatch.ElapsedMilliseconds} ms for path: {directoryPath}");

            return list;
        }

        /// <summary>
        /// Reads all text from a file.
        /// </summary>
        public static string ReadFileText(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

            return File.Exists(filePath)
                ? File.ReadAllText(filePath)
                : string.Empty;
        }

        /// <summary>
        /// Gets the server name and database name from the connection string in web.config in the specified folder.
        /// Returns a tuple (serverName, databaseName), or (null, null) if not found.
        /// </summary>
        public static (string? ServerName, string? DatabaseName) GetServerAndDatabaseFromWebConfig(string folderPath)
        {
            var stopwatch = Stopwatch.StartNew();
            // Search for web.config only in a folder called Site (case-insensitive)

            var siteFolderPath = Path.Combine(folderPath, "WebSites", "Pure", "Site");
            if(!Directory.Exists(siteFolderPath))
                return (null, null);

            string? webConfigPath = null;
            if (siteFolderPath != null)
            {
                webConfigPath = Path.Combine(siteFolderPath, "web.config");
                if (!File.Exists(webConfigPath))
                    webConfigPath = null;
            }

            if (webConfigPath == null)
                return (null, null);

            try
            {
                var doc = XDocument.Load(webConfigPath);
                var connStringElement = doc.Descendants("connectionStrings")
                    .Descendants("add")
                    .FirstOrDefault();

                if (connStringElement == null)
                    return (null, null);

                var connString = connStringElement.Attribute("connectionString")?.Value;
                if (string.IsNullOrWhiteSpace(connString))
                    return (null, null);

                // Parse connection string for server and database
                string? server = null, database = null;
                var parts = connString.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    var kv = part.Split('=', 2);
                    if (kv.Length != 2) continue;
                    var key = kv[0].Trim().ToLowerInvariant();
                    var value = kv[1].Trim();
                    if (key is "server" or "data source")
                        server = value;
                    else if (key is "database" or "initial catalog")
                        database = value;
                }

                stopwatch.Stop();
                Debug.WriteLine($"GetServerAndDatabaseFromWebConfig took {stopwatch.ElapsedMilliseconds} ms for path: {webConfigPath}");
                return (server, database);
            }
            catch
            {
                return (null, null);
            }
        }

        internal static async Task<bool> UpdateWebConfigAsync(string folderPath, string value, IProgress<string> progress)
        {
            // Pseudocode:
            // 1. Build path to web.config in WebSites/Pure/Site under folderPath.
            // 2. Check if file exists, return false if not.
            // 3. Load XML document.
            // 4. Find <connectionStrings>/<add> element.
            // 5. Parse 'value' for new server and database.
            // 6. Update connectionString attribute with new server and database.
            // 7. Save XML document.
            // 8. Report progress and return true if successful.

            var siteFolderPath = Path.Combine(folderPath, "WebSites", "Pure", "Site");
            var webConfigPath = Path.Combine(siteFolderPath, "web.config");

            if (!File.Exists(webConfigPath))
            {
                progress?.Report($"web.config not found at {webConfigPath}");
                return false;
            }

            try
            {
                var doc = XDocument.Load(webConfigPath);
                var connStringElement = doc.Descendants("connectionStrings")
                    .Descendants("add")
                    .FirstOrDefault();

                if (connStringElement == null)
                {
                    progress?.Report("No <add> element found in <connectionStrings>.");
                    return false;
                }

                var connStringAttr = connStringElement.Attribute("connectionString");
                if (connStringAttr == null)
                {
                    progress?.Report("No connectionString attribute found.");
                    return false;
                }

                // Parse new server and database from 'value'                
                var parts = value.Split(';', StringSplitOptions.RemoveEmptyEntries);
                string? newServer = parts[0], newDatabase = parts[1];

                if (string.IsNullOrWhiteSpace(newServer) || string.IsNullOrWhiteSpace(newDatabase))
                {
                    progress?.Report("New server or database not found in value.");
                    return false;
                }

                // Parse and update the existing connection string
                var existingConnString = connStringAttr.Value;
                var connParts = existingConnString.Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Split('=', 2))
                    .Where(kv => kv.Length == 2)
                    .ToDictionary(kv => kv[0].Trim(), kv => kv[1].Trim(), StringComparer.OrdinalIgnoreCase);

                // Update or add server and database
                if (connParts.ContainsKey("Server"))
                    connParts["Server"] = newServer;
                else if (connParts.ContainsKey("Data Source"))
                    connParts["Data Source"] = newServer;
                else
                    connParts["Server"] = newServer;

                if (connParts.ContainsKey("Database"))
                    connParts["Database"] = newDatabase;
                else if (connParts.ContainsKey("Initial Catalog"))
                    connParts["Initial Catalog"] = newDatabase;
                else
                    connParts["Database"] = newDatabase;

                // Rebuild connection string
                var newConnString = string.Join(";", connParts.Select(kv => $"{kv.Key}={kv.Value}"));

                connStringAttr.Value = newConnString;

                // Save changes asynchronously
                using (var stream = File.Open(webConfigPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await Task.Run(() => doc.Save(stream));
                }

                progress?.Report("web.config updated successfully.");
                return true;
            }
            catch (Exception ex)
            {
                progress?.Report($"Error updating web.config: {ex.Message}");
                return false;
            }
        }

        internal static async Task<List<BuildModel>> GetBuildListAsync(string buildPath, IProgress<string> progress)
        {
            // Pseudocode:
            // 1. Validate buildPath.
            // 2. Get all directories in buildPath (major versions).
            // 3. For each major version directory:
            //    a. Get all subdirectories (minor versions).
            //    b. Create BuildModel with MajorVersion and MinorVersions.
            // 4. Return list of BuildModel.

            var result = new List<BuildModel>();

            if (string.IsNullOrWhiteSpace(buildPath) || !Directory.Exists(buildPath))
            {
                progress?.Report($"Build path '{buildPath}' does not exist.");
                return result;
            }

            try
            {
                var majorDirs = Directory.GetDirectories(buildPath);

                foreach (var majorDir in majorDirs)
                {
                    var majorVersion = Path.GetFileName(majorDir);
                    var minorDirs = Directory.GetDirectories(majorDir);
                    var minorVersions = minorDirs
                        .Select(Path.GetFileName)
                        .Where(name => name != null) // Ensure no null values
                        .Cast<string>() // Cast to non-nullable string
                        .ToList();

                    result.Add(new BuildModel
                    {
                        MajorVersion = majorVersion,
                        MinorVersions = minorVersions
                    });

                    progress?.Report($"Found major version '{majorVersion}' with {minorVersions.Count} minor versions.");
                }
            }
            catch (Exception ex)
            {
                progress?.Report($"Error reading build list: {ex.Message}");
            }

            return await Task.FromResult(result);
        }
    }
}