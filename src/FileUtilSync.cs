using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Soenneker.Extensions.FileInfo;
using Soenneker.Extensions.Stream;
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

    /// <summary>
    /// Use this instead of Systems.IO.Path.GetTempFileName()!  <para/>
    /// 1. It creates 0 byte file (so it'll already exist)  <para/>
    /// 2. It's slow because it iterates over the file system to (hopefully) find a non-collision <para/>
    /// https://stackoverflow.com/a/50413126
    /// </summary>
    [Pure]
    public static string GetTempFileName()
    {
        var result = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        return result;
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
        stream.ToStart();

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
        _logger.LogDebug("Checking if file exists: {filename} ...", filename);

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

    public void TryRemoveReadonlyAndArchiveAttributesFromAllFiles(string directory)
    {
        _logger.LogInformation("Trying to remove readonly/archive in {dir} ...", directory);

        List<FileInfo> files = GetAllFileInfoInDirectoryRecursivelySafe(directory);

        foreach (FileInfo file in files)
        {
            try
            {
                file.RemoveReadOnlyOrArchivedAttribute();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not remove attributes from file ({file}), skipping", file.FullName);
            }
        }

        _logger.LogTrace("Completed trying to remove readonly and archive from all files");
    }

    public bool TryRemoveReadonlyAndArchiveAttribute(string fileName)
    {
        try
        {
            var fileInfo = new FileInfo(fileName);
            fileInfo.RemoveReadOnlyOrArchivedAttribute();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not remove attributes from file ({file}), skipping", fileName);
        }

        return false;
    }

    public void TryDeleteAllFiles(string directory)
    {
        _logger.LogInformation("Deleting all files in {dir} ...", directory);

        List<FileInfo> files = GetAllFileInfoInDirectoryRecursivelySafe(directory);

        foreach (FileInfo file in files)
        {
            TryDelete(file.FullName);
        }

        _logger.LogTrace("Completed deleting all files");
    }

    public void DeleteAllFiles(string directory)
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
        string[] allDirectories = Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories);
        foreach (string dir in allDirectories)
        {
            string dirToCreate = dir.Replace(sourceDir, destinationDir);
            Directory.CreateDirectory(dirToCreate);
        }

        List<string> allFiles = GetAllFileNamesInDirectoryRecursively(sourceDir);

        foreach (string newPath in allFiles)
        {
            File.Copy(newPath, newPath.Replace(sourceDir, destinationDir), overwrite);
        }
    }

    [Pure]
    public static long GetFileSize(string path)
    {
        return new FileInfo(path).Length;
    }

    public List<string> GetAllFileNamesInDirectoryRecursively(string directory)
    {
        _logger.LogDebug("Getting all files from directory ({directory}) recursively...", directory);

        List<string> result = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).ToList();

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
        List<string> allFiles = GetAllFileNamesInDirectoryRecursively(sourceDirectory);

        foreach (string file in allFiles)
        {
            string newFileName = file.Replace(oldValue, newValue);
            Move(file, newFileName);
        }
    }
}