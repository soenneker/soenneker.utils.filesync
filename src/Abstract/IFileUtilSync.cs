using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;

namespace Soenneker.Utils.FileSync.Abstract;

/// <summary>
/// Provides synchronous file operations with optional logging.
/// </summary>
public interface IFileUtilSync
{
    /// <summary>
    /// Reads all text from the specified file.
    /// </summary>
    /// <param name="path">The file path to read from.</param>
    /// <param name="log">Whether to log the operation.</param>
    /// <returns>The file contents as a string.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
    /// <exception cref="IOException">Thrown on I/O error.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown if access is denied.</exception>
    string Read(string path, bool log = true);

    /// <summary>
    /// Reads all bytes from the specified file.
    /// </summary>
    /// <param name="path">The file path to read from.</param>
    /// <param name="log">Whether to log the operation.</param>
    /// <returns>The file contents as a byte array.</returns>
    /// <exception cref="FileNotFoundException" />
    /// <exception cref="IOException" />
    /// <exception cref="UnauthorizedAccessException" />
    byte[] ReadToBytes(string path, bool log = true);

    /// <summary>
    /// Reads all lines from the specified file.
    /// </summary>
    /// <param name="path">The file path to read from.</param>
    /// <param name="log">Whether to log the operation.</param>
    /// <returns>A list of lines from the file.</returns>
    /// <exception cref="FileNotFoundException" />
    /// <exception cref="IOException" />
    /// <exception cref="UnauthorizedAccessException" />
    List<string> ReadAsLines(string path, bool log = true);

    /// <summary>
    /// Writes the specified lines to a file.
    /// </summary>
    /// <param name="path">The file path to write to.</param>
    /// <param name="lines">The lines to write.</param>
    /// <param name="log">Whether to log the operation.</param>
    /// <exception cref="IOException" />
    /// <exception cref="UnauthorizedAccessException" />
    void WriteAllLines(string path, IEnumerable<string> lines, bool log = true);

    /// <summary>
    /// Writes text content to a file.
    /// </summary>
    /// <param name="path">The file path to write to.</param>
    /// <param name="content">The text content to write.</param>
    /// <param name="log">Whether to log the operation.</param>
    /// <exception cref="IOException" />
    /// <exception cref="UnauthorizedAccessException" />
    void Write(string path, string content, bool log = true);

    /// <summary>
    /// Writes a stream to a file.
    /// </summary>
    /// <param name="path">The file path to write to.</param>
    /// <param name="stream">The stream to write.</param>
    /// <param name="log">Whether to log the operation.</param>
    void Write(string path, Stream stream, bool log = true);

    /// <summary>
    /// Writes a byte array to a file.
    /// </summary>
    /// <param name="path">The file path to write to.</param>
    /// <param name="byteArray">The data to write.</param>
    /// <param name="log">Whether to log the operation.</param>
    void Write(string path, byte[] byteArray, bool log = true);

    /// <summary>
    /// Determines whether the specified file exists.
    /// </summary>
    /// <param name="filename">The file path to check.</param>
    /// <param name="log">Whether to log the check.</param>
    /// <returns>True if the file exists; otherwise, false.</returns>
    bool Exists(string filename, bool log = true);

    /// <summary>
    /// Deletes the specified file.
    /// </summary>
    /// <param name="filename">The file path to delete.</param>
    /// <param name="log">Whether to log the operation.</param>
    void Delete(string filename, bool log = true);

    /// <summary>
    /// Deletes multiple files.
    /// </summary>
    /// <param name="files">The list of file infos to delete.</param>
    /// <param name="parallel">Whether to delete in parallel.</param>
    /// <param name="log">Whether to log the operation.</param>
    void Delete(List<FileInfo> files, bool parallel = false, bool log = true);

    /// <summary>
    /// Deletes the file if it exists.
    /// </summary>
    /// <param name="filename">The file path to delete.</param>
    /// <param name="log">Whether to log the operation.</param>
    /// <returns>True if the file was deleted; otherwise, false.</returns>
    bool DeleteIfExists(string filename, bool log = true);

    /// <summary>
    /// Tries to delete the file if it exists, catching any exceptions.
    /// </summary>
    /// <param name="filename">The file path to delete.</param>
    /// <param name="log">Whether to log the operation.</param>
    /// <returns>True if the file was deleted; otherwise, false.</returns>
    bool TryDeleteIfExists(string filename, bool log = true);

    /// <summary>
    /// Removes read-only and archive attributes from all files in a directory.
    /// </summary>
    /// <param name="directory">The directory path.</param>
    /// <param name="log">Whether to log the operation.</param>
    void TryRemoveReadonlyAndArchiveAttributesFromAll(string directory, bool log = true);

    /// <summary>
    /// Removes read-only and archive attributes from a single file.
    /// </summary>
    /// <param name="fileName">The file path.</param>
    /// <param name="log">Whether to log the operation.</param>
    /// <returns>True if attributes were removed; otherwise, false.</returns>
    bool TryRemoveReadonlyAndArchiveAttribute(string fileName, bool log = true);

    /// <summary>
    /// Attempts to delete all files in a directory, catching exceptions.
    /// </summary>
    /// <param name="directory">The directory path.</param>
    /// <param name="log">Whether to log the operation.</param>
    void TryDeleteAll(string directory, bool log = true);

    /// <summary>
    /// Deletes all files in a directory.
    /// </summary>
    /// <param name="directory">The directory path.</param>
    /// <param name="log">Whether to log the operation.</param>
    void DeleteAll(string directory, bool log = true);

    /// <summary>
    /// Tries to delete a file, catching and logging any exceptions.
    /// </summary>
    /// <param name="filename">The file path to delete.</param>
    /// <param name="log">Whether to log the operation.</param>
    /// <returns>True if the file was deleted; otherwise, false.</returns>
    bool TryDelete(string filename, bool log = true);

    /// <summary>
    /// Moves a file from source to target.
    /// </summary>
    /// <param name="source">The source file path.</param>
    /// <param name="target">The target file path.</param>
    /// <param name="log">Whether to log the operation.</param>
    void Move(string source, string target, bool log = true);

    /// <summary>
    /// Copies a file from source to target.
    /// </summary>
    /// <param name="source">The source file path.</param>
    /// <param name="target">The target file path.</param>
    /// <param name="log">Whether to log the operation.</param>
    void Copy(string source, string target, bool log = true);

    /// <summary>
    /// Tries to copy a file, catching and logging any exceptions.
    /// </summary>
    /// <param name="source">The source file path.</param>
    /// <param name="target">The target file path.</param>
    /// <param name="log">Whether to log the operation.</param>
    /// <returns>True if the file was copied; otherwise, false.</returns>
    bool TryCopy(string source, string target, bool log = true);

    /// <summary>
    /// Recursively copies a directory.
    /// </summary>
    /// <param name="sourceDir">The source directory.</param>
    /// <param name="destinationDir">The destination directory.</param>
    /// <param name="overwrite">Whether to overwrite existing files.</param>
    /// <param name="log">Whether to log the operation.</param>
    void CopyRecursively(string sourceDir, string destinationDir, bool overwrite = true, bool log = true);

    /// <summary>
    /// Copies files in a single directory.
    /// </summary>
    /// <param name="sourceDirectory">The source directory.</param>
    /// <param name="destinationDirectory">The destination directory.</param>
    /// <param name="overwrite">Whether to overwrite existing files.</param>
    /// <param name="log">Whether to log the operation.</param>
    void CopyDirectory(string sourceDirectory, string destinationDirectory, bool overwrite = true, bool log = true);

    /// <summary>
    /// Gets all file names in a directory recursively.
    /// </summary>
    /// <param name="directory">The directory path.</param>
    /// <param name="log">Whether to log the operation.</param>
    /// <returns>An array of file paths.</returns>
    string[] GetAllFileNamesInDirectoryRecursively(string directory, bool log = true);

    /// <summary>
    /// Safely gets all FileInfo objects in a directory recursively.
    /// </summary>
    /// <param name="directory">The directory path.</param>
    /// <param name="log">Whether to log the operation.</param>
    /// <returns>A list of FileInfo objects.</returns>
    List<FileInfo> GetAllFileInfoInDirectoryRecursivelySafe(string directory, bool log = true);

    /// <summary>
    /// Renames all matching occurrences in file paths recursively.
    /// </summary>
    /// <param name="sourceDirectory">The directory to scan.</param>
    /// <param name="oldValue">The substring to replace.</param>
    /// <param name="newValue">The replacement substring.</param>
    /// <param name="log">Whether to log the operation.</param>
    void RenameAllInDirectoryRecursively(string sourceDirectory, string oldValue, string newValue, bool log = true);
}