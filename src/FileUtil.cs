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

    public long GetFileSize(string path)
    {
        return new FileInfo(path).Length;
    }
}