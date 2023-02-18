using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Soenneker.Utils.FileSync.Abstract;

namespace Soenneker.Utils.FileSync;

///<inheritdoc cref="IFileUtilSync"/>
public class FileUtilSync : IFileUtilSync
{
    private readonly ILogger<FileUtilSync> _logger;

    public FileUtilSync(ILogger<FileUtilSync> logger)
    {
        _logger = logger;
    }

    public string GetTempFileName()
    {
        return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    }

    public string ReadFile(string path)
    {
        _logger.LogDebug("{name} start for {path} ...", nameof(File.ReadAllText), path);

        string result = File.ReadAllText(path);
        return result;
    }

    public byte[] ReadFileToBytes(string path)
    {
        _logger.LogDebug("{name} start for {path} ...", nameof(File.ReadAllBytes), path);

        byte[] result = File.ReadAllBytes(path);

        return result;
    }

    public List<string> ReadFileAsLines(string path)
    {
        _logger.LogDebug("{name} start for {name} ...", nameof(File.ReadAllLines), path);

        List<string> content = File.ReadAllLines(path).ToList();

        return content;
    }

    public void WriteAllLines(string path, IEnumerable<string> lines)
    {
        _logger.LogDebug("{name} start for {path} ...", nameof(File.WriteAllLines), path);

        File.WriteAllLines(path, lines);
    }

    public void WriteFile(string path, string content)
    {
        _logger.LogDebug("{name} start for {name} ...", nameof(File.WriteAllText), path);

        File.WriteAllText(path, content);
    }

    public void WriteFile(string path, Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);

        using (var fs = new FileStream(path, FileMode.OpenOrCreate))
        {
            stream.CopyTo(fs);
        }
    }

    public void WriteFile(string path, byte[] byteArray)
    {
        _logger.LogDebug("{name} start for {name} ...", nameof(File.WriteAllBytes), path);

        File.WriteAllBytes(path, byteArray);
    }

    public bool Exists(string filename)
    {
        _logger.LogDebug("Checking if file exists: {filename}...", filename);

        if (!File.Exists(filename))
        {
            _logger.LogDebug("{filename} does not exist", filename);
            return false;
        }

        _logger.LogDebug("File exists: {filename}", filename);

        return true;
    }

    public void Delete(string filename)
    {
        _logger.LogDebug("Deleting {filename} ...", filename);
        File.Delete(filename);
    }

    public bool DeleteIfExists(string filename)
    {
        _logger.LogDebug("Deleting file if it exists: {filename} ...", filename);

        if (Exists(filename))
            return false;

        Delete(filename);

        return true;
    }

    public void DeleteAllFilesSafe(string directory)
    {
        _logger.LogInformation("Deleting all files in {directory} ...", directory);

        List<FileInfo> files = GetAllFileInfoInDirectoryRecursivelySafe(directory);

        foreach (FileInfo file in files)
        {
            Delete(file.FullName);
        }

        _logger.LogDebug("Completed deleting all files from {directory}", directory);
    }

    public bool TryDelete(string filename)
    {
        _logger.LogDebug("Trying to delete {filename} ...", filename);

        try
        {
            File.Delete(filename);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception deleting {filename}", filename);
            return false;
        }
    }

    public void Move(string source, string target)
    {
        if (source == target)
        {
            _logger.LogWarning("Not moving file ({source}) because source = target", source);
            return;
        }

        _logger.LogDebug("Moving {source} to {target} ...", source, target);

        File.Move(source, target);

        _logger.LogDebug("Finished moving {source} to {target}", source, target);
    }

    public void Copy(string source, string target)
    {
        _logger.LogDebug("Copying {source} to {target} ...", source, target);

        File.Copy(source, target);

        _logger.LogDebug("Finished copying {source} to {target}", source, target);
    }

    public bool TryCopy(string source, string target)
    {
        _logger.LogDebug("Trying to copy {source} to {target} ...", source, target);

        try
        {
            File.Copy(source, target);
            _logger.LogDebug("Finished copying {source} to {target}", source, target);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception copying {source} to {target}", source, target);
            return false;
        }
    }

    public void CopyFilesRecursively(string sourceDir, string destinationDir, bool overwrite = true)
    {
        // Copy the directory structure
        var allDirectories = Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories);
        foreach (var dir in allDirectories)
        {
            var dirToCreate = dir.Replace(sourceDir, destinationDir);
            Directory.CreateDirectory(dirToCreate);
        }

        var allFiles = GetAllFileNamesInDirectoryRecursively(sourceDir);

        foreach (var newPath in allFiles)
        {
            File.Copy(newPath, newPath.Replace(sourceDir, destinationDir), overwrite);
        }
    }

    public long GetFileSize(string path)
    {
        return new FileInfo(path).Length;
    }

    public List<string> GetAllFileNamesInDirectoryRecursively(string directory)
    {
        _logger.LogDebug("Getting all files from directory ({directory}) recursively...", directory);

        var result = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).ToList();

        return result;
    }

    public List<FileInfo> GetAllFileInfoInDirectoryRecursivelySafe(string directory)
    {
        _logger.LogDebug("Getting all FileInfos in {directory} recursively...", directory);

        var list = new List<FileInfo>();

        try
        {
            var diTop = new DirectoryInfo(directory);

            // Get all files in top level
            foreach (FileInfo fi in diTop.EnumerateFiles())
            {
                try
                {
                    var fileInfo = new FileInfo(fi.FullName);
                    list.Add(fileInfo);
                }
                catch (UnauthorizedAccessException)
                {
                    _logger.LogWarning("Unauthorized Exception for {fullName}", fi.FullName);
                }
            }

            // Get each subdirectory, and then enumerate over those
            foreach (DirectoryInfo? di in diTop.EnumerateDirectories("*"))
            {
                try
                {
                    foreach (FileInfo? fi in di.EnumerateFiles("*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            var fileInfo = new FileInfo(fi.FullName);
                            list.Add(fileInfo);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            _logger.LogWarning("Unauthorized Exception for {fullName}", fi.FullName);
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    _logger.LogWarning("Unauthorized Exception for {fullName}", di.FullName);
                }
            }
        }
        catch (DirectoryNotFoundException dirNotFound)
        {
            _logger.LogWarning("DirectoryNotFoundException {message}", dirNotFound.Message);
        }
        catch (UnauthorizedAccessException unAuthDir)
        {
            _logger.LogWarning("UnauthorizedAccessException: {message}", unAuthDir.Message);
        }
        catch (PathTooLongException longPath)
        {
            _logger.LogWarning("PathTooLongException {message}", longPath.Message);
        }

        _logger.LogDebug("Completed getting all files in {dir}, number: {number}", directory, list.Count);

        return list;
    }

    public void RenameAllFilesInDirectoryRecursively(string sourceDirectory, string oldValue, string newValue)
    {
        var allFiles = GetAllFileNamesInDirectoryRecursively(sourceDirectory);

        foreach (var file in allFiles)
        {
            var newFileName = file.Replace(oldValue, newValue);
            Move(file, newFileName);
        }
    }
}