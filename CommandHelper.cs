using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AcuEnvManager
{
    public static class CommandHelper
    {
        /// <summary>
        /// Gets the current branch name of the git repository at the specified path.
        /// </summary>
        public static async Task<string?> GetCurrentBranchAsync(string repoPath, IProgress<string> progress)
        {
            (bool success, string? output) = await RunGitCommandAsync("rev-parse --abbrev-ref HEAD", repoPath, progress);
            return output;
        }

        /// <summary>
        /// Gets the latest commit hash of the git repository at the specified path.
        /// </summary>
        public static string? GetLatestCommitHash(string repoPath)
        {
            return RunGitCommand("rev-parse HEAD", repoPath);
        }

        /// <summary>
        /// Gets the most recent tag name in the git repository at the specified path.
        /// </summary>
        public static async Task<string?> GetLatestTagAsync(string repoPath, IProgress<string> progress)
        {
            (bool success, string? output) = await RunGitCommandAsync("describe --tags --abbrev=0", repoPath, progress);
            return output;
        }

        /// <summary>
        /// Checks if the specified path is a git repository.
        /// </summary>
        public static bool IsGitRepository(string repoPath)
        {
            var stopwatch = Stopwatch.StartNew();
            var exists = File.Exists(Path.Combine(repoPath, ".git"));
            stopwatch.Stop();
            Debug.WriteLine($"IsGitRepository check took {stopwatch.ElapsedMilliseconds} ms for path: {repoPath}");
            return exists;
        }

        internal static async Task<bool> CheckOutBranchAsync(string workingDirectory, string branchname, IProgress<string> progress)
        {     
            (bool success, string? output) = await RunGitCommandAsync($"checkout {branchname}", workingDirectory, progress);
            return success;
        }

        internal static async Task<bool> AcubuildAsync(string workingdirectory, IProgress<string> progress)
        {
            var (exitCode, result) = await RunCommandAsync("acubuild", "", workingdirectory, progress);
            return exitCode == 0;
        }

        /// <summary>
        /// Runs a command prompt command asynchronously and returns the output.
        /// </summary>
        /// <param name="command">The command to run (e.g., "dir").</param>
        /// <param name="arguments">Arguments for the command (optional).</param>
        /// <param name="workingDirectory">Working directory (optional).</param>
        /// <returns>Standard output and error as a string.</returns>
        public static async Task<(int exitCode, string output)> RunCommandAsync(string command, string arguments, string? workingDirectory, IProgress<string> progress)
        {
            var output = new StringBuilder();

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory ?? string.Empty,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.OutputDataReceived += (s, e) => { if (e.Data != null) progress.Report(e.Data); output.AppendLine(e.Data); };
            process.ErrorDataReceived += (s, e) => { if (e.Data != null) progress.Report(e.Data); output.AppendLine(e.Data); };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync();

            return (process.ExitCode, output.ToString());
        }

        internal static async Task<(bool, string?)> RunGitCommandAsync(string arguments, string workingDirectory, IProgress<string> progress)
        {
            bool result = true;
            string output = string.Empty;
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processStartInfo, EnableRaisingEvents = true };
            process.Start();

            // Read output and error streams asynchronously
            var outputTask = Task.Run(async () =>
            {
                while (!process.StandardOutput.EndOfStream)
                {
                    var line = await process.StandardOutput.ReadLineAsync();
                    if (!string.IsNullOrEmpty(line))
                    {
                        progress.Report(line);
                        output += line;
                    }
                }
            });

            var errorTask = Task.Run(async () =>
            {
                while (!process.StandardError.EndOfStream)
                {
                    var line = await process.StandardError.ReadLineAsync();
                    if (!string.IsNullOrEmpty(line))
                    {
                        progress.Report(line);
                        output += line;
                    }
                }
            });

            await Task.WhenAll(outputTask, errorTask);
            await process.WaitForExitAsync();
            return (result, output);
        }

        private static string? RunGitCommand(string arguments, string workingDirectory)
        {
            var stopwatch = Stopwatch.StartNew();   
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(processStartInfo);
                if (process == null)
                    return null;

                string output = process.StandardOutput.ReadLine() ?? string.Empty;
                process.WaitForExit();
                stopwatch.Stop();
                Debug.WriteLine($"Git command '{arguments}' took {stopwatch.ElapsedMilliseconds} ms in directory: {workingDirectory}");
                return string.IsNullOrWhiteSpace(output) ? null : output.Trim();
            }
            catch
            {
                return null;
            }
        }


    }
}