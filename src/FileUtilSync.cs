using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Soenneker.Extensions.FileInfo;
using Soenneker.Extensions.Stream;
using Soenneker.Utils.Directory.Abstract;
using Soenneker.Utils.FileSync.Abstract;

namespace Soenneker.Utils.FileSync;

///<inheritdoc cref="IFileUtilSync"/>
public class FileUtilSync : IFileUtilSync
{
    private readonly ILogger<FileUtilSync> _logger;
    private readonly IDirectoryUtil _directoryUtil;

    public FileUtilSync(ILogger<FileUtilSync> logger, IDirectoryUtil directoryUtil)
    {
        _logger = logger;
        _directoryUtil = directoryUtil;
    }

    [Pure]
    public static string GetTempFileName() => System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());

    public string Read(string path, bool log = true)
    {
        if (log) _logger.LogDebug("{name} start for {path} ...", nameof(File.ReadAllText), path);
        return File.ReadAllText(path);
    }

    public byte[] ReadToBytes(string path, bool log = true)
    {
        if (log) _logger.LogDebug("{name} start for {path} ...", nameof(File.ReadAllBytes), path);
        return File.ReadAllBytes(path);
    }

    public List<string> ReadAsLines(string path, bool log = true)
    {
        if (log) _logger.LogDebug("{name} start for {path} ...", nameof(File.ReadAllLines), path);
        return File.ReadAllLines(path).ToList();
    }

    public void WriteAllLines(string path, IEnumerable<string> lines, bool log = true)
    {
        if (log) _logger.LogDebug("{name} start for {path} ...", nameof(File.WriteAllLines), path);
        File.WriteAllLines(path, lines);
    }

    public void Write(string path, string content, bool log = true)
    {
        if (log) _logger.LogDebug("{name} start for {path} ...", nameof(File.WriteAllText), path);
        File.WriteAllText(path, content);
    }

    public void Write(string path, Stream stream, bool log = true)
    {
        if (log) _logger.LogDebug("Writing stream to {path} ...", path);
        stream.ToStart();
        using var fs = new FileStream(path, FileMode.OpenOrCreate);
        stream.CopyTo(fs);
    }

    public void Write(string path, byte[] byteArray, bool log = true)
    {
        if (log) _logger.LogDebug("{name} start for {path} ...", nameof(File.WriteAllBytes), path);
        File.WriteAllBytes(path, byteArray);
    }

    public bool Exists(string filename, bool log = true)
    {
        if (log) _logger.LogDebug("Checking if file exists: {filename} ...", filename);
        bool exists = File.Exists(filename);
        if (log) _logger.LogDebug(exists ? "File exists: {filename}" : "{filename} does not exist", filename);
        return exists;
    }

    public void Delete(string filename, bool log = true)
    {
        if (log)
            _logger.LogDebug("Deleting {filename} ...", filename);
        File.Delete(filename);
    }

    public void Delete(List<FileInfo> files, bool parallel = false, bool log = true)
    {
        if (parallel)
            Parallel.For(0, files.Count, i => Delete(files[i].FullName, log));
        else
            foreach (FileInfo file in files)
                Delete(file.FullName, log);
    }

    public bool DeleteIfExists(string filename, bool log = true)
    {
        if (log) _logger.LogDebug("Deleting file if it exists: {filename} ...", filename);
        if (!Exists(filename, log)) return false;
        Delete(filename, log);
        return true;
    }

    public bool TryDeleteIfExists(string filename, bool log = true)
    {
        if (log) _logger.LogDebug("Trying to delete file if it exists: {filename} ...", filename);
        if (!Exists(filename, log)) return false;
        return TryDelete(filename, log);
    }

    public void TryRemoveReadonlyAndArchiveAttributesFromAll(string directory, bool log = true)
    {
        if (log) _logger.LogInformation("Trying to remove readonly/archive in {dir} ...", directory);
        List<FileInfo> files = GetAllFileInfoInDirectoryRecursivelySafe(directory, log);
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

        if (log) _logger.LogTrace("Completed trying to remove readonly and archive from all files");
    }

    public bool TryRemoveReadonlyAndArchiveAttribute(string fileName, bool log = true)
    {
        try
        {
            new FileInfo(fileName).RemoveReadOnlyOrArchivedAttribute();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not remove attributes from file ({file}), skipping", fileName);
            return false;
        }
    }

    public void TryDeleteAll(string directory, bool log = true)
    {
        if (log) _logger.LogInformation("Deleting all files in {dir} ...", directory);
        List<FileInfo> files = GetAllFileInfoInDirectoryRecursivelySafe(directory, log);
        foreach (FileInfo file in files)
            TryDelete(file.FullName, log);
        if (log) _logger.LogTrace("Completed deleting all files");
    }

    public void DeleteAll(string directory, bool log = true)
    {
        if (log) _logger.LogInformation("Deleting all files in {directory} ...", directory);
        List<FileInfo> files = GetAllFileInfoInDirectoryRecursivelySafe(directory, log);
        foreach (FileInfo file in files)
            Delete(file.FullName, log);
        if (log) _logger.LogDebug("Completed deleting all files from {directory}", directory);
    }

    public bool TryDelete(string filename, bool log = true)
    {
        if (log) _logger.LogDebug("Trying to delete {filename} ...", filename);
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

    public void Move(string source, string target, bool log = true)
    {
        if (source == target)
        {
            if (log) _logger.LogWarning("Not moving file ({source}) because source = target", source);
            return;
        }

        if (log) _logger.LogDebug("Moving {source} to {target} ...", source, target);
        File.Move(source, target);
        if (log) _logger.LogDebug("Finished moving {source} to {target}", source, target);
    }

    public void Copy(string source, string target, bool log = true)
    {
        if (log) _logger.LogDebug("Copying {source} to {target} ...", source, target);
        File.Copy(source, target);
        if (log) _logger.LogDebug("Finished copying {source} to {target}", source, target);
    }

    public bool TryCopy(string source, string target, bool log = true)
    {
        if (log) _logger.LogDebug("Trying to copy {source} to {target} ...", source, target);
        try
        {
            File.Copy(source, target);
            if (log) _logger.LogDebug("Finished copying {source} to {target}", source, target);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception copying {source} to {target}", source, target);
            return false;
        }
    }

    public void CopyRecursively(string sourceDir, string destinationDir, bool overwrite = true, bool log = true)
    {
        if (log) _logger.LogDebug("Copying directory {sourceDir} to {destinationDir}...", sourceDir, destinationDir);
        string[] allDirectories = System.IO.Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories);
        foreach (string dir in allDirectories)
        {
            string dirToCreate = dir.Replace(sourceDir, destinationDir);
            System.IO.Directory.CreateDirectory(dirToCreate);
        }

        string[] allFiles = GetAllFileNamesInDirectoryRecursively(sourceDir, log);
        foreach (string newPath in allFiles)
        {
            string destPath = newPath.Replace(sourceDir, destinationDir);
            File.Copy(newPath, destPath, overwrite);
        }
    }

    public void CopyDirectory(string sourceDirectory, string destinationDirectory, bool overwrite = true, bool log = true)
    {
        if (!System.IO.Directory.Exists(sourceDirectory)) throw new Exception($"Source directory ({sourceDirectory}) does not exist");
        if (log) _ = _directoryUtil.CreateIfDoesNotExist(destinationDirectory);
        string[] files = System.IO.Directory.GetFiles(sourceDirectory);
        foreach (string filePath in files)
        {
            string fileName = System.IO.Path.GetFileName(filePath);
            string destinationPath = System.IO.Path.Combine(destinationDirectory, fileName);
            File.Copy(filePath, destinationPath, overwrite);
        }
    }

    [Pure]
    public static long GetSize(string path) => new FileInfo(path).Length;

    public string[] GetAllFileNamesInDirectoryRecursively(string directory, bool log = true)
    {
        if (log) _logger.LogDebug("Getting all files from directory ({directory}) recursively...", directory);
        return System.IO.Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
    }

    public List<FileInfo> GetAllFileInfoInDirectoryRecursivelySafe(string directory, bool log = true)
    {
        if (log) _logger.LogDebug("Getting all FileInfos in {directory} recursively...", directory);
        var list = new List<FileInfo>();
        try
        {
            var diTop = new DirectoryInfo(directory);
            foreach (FileInfo fi in diTop.EnumerateFiles())
            {
                try
                {
                    list.Add(new FileInfo(fi.FullName));
                }
                catch (UnauthorizedAccessException)
                {
                    _logger.LogWarning("Unauthorized Exception for {fullName}", fi.FullName);
                }
            }

            foreach (DirectoryInfo di in diTop.EnumerateDirectories("*"))
            {
                try
                {
                    foreach (FileInfo fi in di.EnumerateFiles("*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            list.Add(new FileInfo(fi.FullName));
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
        catch (Exception e) when (e is DirectoryNotFoundException || e is UnauthorizedAccessException || e is PathTooLongException)
        {
            _logger.LogWarning(e, e.Message);
        }

        if (log) _logger.LogDebug("Completed getting all files in {directory}, number: {number}", directory, list.Count);
        return list;
    }

    public void RenameAllInDirectoryRecursively(string sourceDirectory, string oldValue, string newValue, bool log = true)
    {
        string[] allFiles = GetAllFileNamesInDirectoryRecursively(sourceDirectory, log);
        foreach (string file in allFiles)
        {
            string newFileName = file.Replace(oldValue, newValue);
            Move(file, newFileName, log);
        }
    }
}