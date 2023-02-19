using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;

namespace Soenneker.Utils.FileSync.Abstract;

/// <summary>
/// A utility library encapsulating synchronous file IO operations <para/>
/// Typically, you'll want to use <code>Soenneker.Utils.File</code> for reading/writing (which is async)
/// </summary>
public interface IFileUtilSync
{
    /// <summary>
    /// Use this instead of Systems.IO.Path.GetTempFileName()!  <para/>
    /// 1. It creates 0 byte file (so it'll already exist)  <para/>
    /// 2. It's slow because it iterates over the file system to (hopefully) find a non-collision <para/>
    /// https://stackoverflow.com/a/50413126
    /// </summary>
    [Pure]
    string GetTempFileName();

    [Pure]
    string ReadFile(string path);

    [Pure]
    byte[] ReadFileToBytes(string path);

    [Pure]
    List<string> ReadFileAsLines(string path);

    void WriteFile(string fullName, string content);

    void WriteAllLines(string path, IEnumerable<string> content);

    /// <summary>
    /// Seeks to the start before writing, so no need before calling this. <para/>
    /// Closes file after writing. Does NOT close stream after writing.
    /// </summary>
    void WriteFile(string path, Stream stream);

    void WriteFile(string path, byte[] byteArray);

    [Pure]
    bool Exists(string filename);

    void Delete(string filename);

    bool DeleteIfExists(string filename);

    bool TryDelete(string filename);

    void Move(string source, string target);

    void Copy(string source, string target);

    bool TryCopy(string source, string target);

    [Pure]
    long GetFileSize(string path);

    [Pure]
    List<string> GetAllFileNamesInDirectoryRecursively(string directory);

    void RenameAllFilesInDirectoryRecursively(string sourceDirectory, string oldValue, string newValue);

    [Pure]
    List<FileInfo> GetAllFileInfoInDirectoryRecursivelySafe(string directory);

    void TryDeleteAllFiles(string directory);

    void DeleteAllFiles(string directory);

    void CopyFilesRecursively(string sourceDir, string destinationDir, bool overwrite = true);
}