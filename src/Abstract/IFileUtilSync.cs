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
    /// Reads the content of a file and returns it as a string.
    /// </summary>
    /// <param name="path">The path of the file to read.</param>
    /// <returns>The content of the file as a string.</returns>
    [Pure]
    string Read(string path);

    /// <summary>
    /// Reads the content of a file and returns it as a byte array.
    /// </summary>
    /// <param name="path">The path of the file to read.</param>
    /// <returns>The content of the file as a byte array.</returns>
    [Pure]
    byte[] ReadToBytes(string path);

    /// <summary>
    /// Reads the content of a file and returns it as a list of lines.
    /// </summary>
    /// <param name="path">The path of the file to read.</param>
    /// <returns>A list of strings, each representing a line in the file.</returns>
    [Pure]
    List<string> ReadAsLines(string path);

    /// <summary>
    /// Writes the specified content to a file.
    /// </summary>
    /// <param name="fullName">The full path of the file to write to.</param>
    /// <param name="content">The content to write to the file.</param>
    void Write(string fullName, string content);

    /// <summary>
    /// Writes a collection of strings to a file, each string as a new line.
    /// </summary>
    /// <param name="path">The path of the file to write to.</param>
    /// <param name="content">The collection of strings to write to the file.</param>
    void WriteAllLines(string path, IEnumerable<string> content);

    /// <summary>
    /// Writes the content of a stream to a file. Seeks to the start before writing and closes the file after writing. Does NOT close the stream after writing.
    /// </summary>
    /// <param name="path">The path of the file to write to.</param>
    /// <param name="stream">The stream whose content is to be written to the file.</param>
    void Write(string path, Stream stream);

    /// <summary>
    /// Writes a byte array to a file.
    /// </summary>
    /// <param name="path">The path of the file to write to.</param>
    /// <param name="byteArray">The byte array to write to the file.</param>
    void Write(string path, byte[] byteArray);

    /// <summary>
    /// Checks if a file exists at the specified path.
    /// </summary>
    /// <param name="filename">The path of the file to check.</param>
    /// <returns>True if the file exists; otherwise, false.</returns>
    [Pure]
    bool Exists(string filename);

    /// <summary>
    /// Deletes the specified file.
    /// </summary>
    /// <param name="filename">The path of the file to delete.</param>
    void Delete(string filename);

    /// <summary>
    /// Deletes a list of files. Optionally performs deletions in parallel.
    /// </summary>
    /// <param name="files">The list of files to delete.</param>
    /// <param name="parallel">If true, deletes files in parallel.</param>
    void Delete(List<FileInfo> files, bool parallel = false);

    /// <summary>
    /// Deletes the specified file if it exists.
    /// </summary>
    /// <param name="filename">The path of the file to delete if it exists.</param>
    /// <returns>True if the file was deleted; otherwise, false.</returns>
    bool DeleteIfExists(string filename);

    /// <summary>
    /// Tries to delete the specified file if it exists, catching any exceptions.
    /// </summary>
    /// <param name="filename">The path of the file to delete if it exists.</param>
    /// <returns>True if the file was successfully deleted; otherwise, false.</returns>
    bool TryDeleteIfExists(string filename);

    /// <summary>
    /// Tries to delete the specified file, catching any exceptions.
    /// </summary>
    /// <param name="filename">The path of the file to delete.</param>
    /// <returns>True if the file was successfully deleted; otherwise, false.</returns>
    bool TryDelete(string filename);

    /// <summary>
    /// Moves a file from the source path to the target path.
    /// </summary>
    /// <param name="source">The path of the file to move.</param>
    /// <param name="target">The path to move the file to.</param>
    void Move(string source, string target);

    /// <summary>
    /// Copies a file from the source path to the target path.
    /// </summary>
    /// <param name="source">The path of the file to copy.</param>
    /// <param name="target">The path to copy the file to.</param>
    void Copy(string source, string target);

    /// <summary>
    /// Copies all files from the source directory to the destination directory.
    /// </summary>
    /// <param name="sourceDirectory">The path of the directory to copy files from.</param>
    /// <param name="destinationDirectory">The path of the directory to copy files to.</param>
    /// <param name="overwrite">If true, overwrites existing files in the destination directory.</param>
    void CopyDirectory(string sourceDirectory, string destinationDirectory, bool overwrite = true);

    /// <summary>
    /// Tries to copy a file from the source path to the target path, catching any exceptions.
    /// </summary>
    /// <param name="source">The path of the file to copy.</param>
    /// <param name="target">The path to copy the file to.</param>
    /// <returns>True if the file was successfully copied; otherwise, false.</returns>
    bool TryCopy(string source, string target);

    /// <summary>
    /// Gets all file names in the specified directory and its subdirectories.
    /// </summary>
    /// <param name="directory">The path of the directory to search.</param>
    /// <returns>A list of all file names in the directory and its subdirectories.</returns>
    [Pure]
    string[] GetAllFileNamesInDirectoryRecursively(string directory);

    /// <summary>
    /// Renames all files in the specified directory and its subdirectories by replacing an old value with a new value in the file names.
    /// </summary>
    /// <param name="sourceDirectory">The path of the directory containing the files to rename.</param>
    /// <param name="oldValue">The value to replace in the file names.</param>
    /// <param name="newValue">The new value to insert in the file names.</param>
    void RenameAllInDirectoryRecursively(string sourceDirectory, string oldValue, string newValue);

    /// <summary>
    /// Gets all file information objects in the specified directory and its subdirectories, handling any exceptions.
    /// </summary>
    /// <param name="directory">The path of the directory to search.</param>
    /// <returns>A list of file information objects for all files in the directory and its subdirectories.</returns>
    [Pure]
    List<FileInfo> GetAllFileInfoInDirectoryRecursivelySafe(string directory);

    /// <summary>
    /// Tries to delete all files in the specified directory, handling any exceptions.
    /// </summary>
    /// <param name="directory">The path of the directory whose files are to be deleted.</param>
    void TryDeleteAll(string directory);

    /// <summary>
    /// Deletes all files in the specified directory.
    /// </summary>
    /// <param name="directory">The path of the directory whose files are to be deleted.</param>
    void DeleteAll(string directory);

    /// <summary>
    /// Copies all files from the source directory to the destination directory, including subdirectories.
    /// </summary>
    /// <param name="sourceDir">The path of the source directory.</param>
    /// <param name="destinationDir">The path of the destination directory.</param>
    /// <param name="overwrite">If true, overwrites existing files in the destination directory.</param>
    void CopyRecursively(string sourceDir, string destinationDir, bool overwrite = true);

    /// <summary>
    /// Tries to remove the read-only and archive attributes from a file.
    /// </summary>
    /// <param name="fileName">The path of the file whose attributes are to be modified.</param>
    /// <returns>True if the attributes were successfully removed; otherwise, false.</returns>
    bool TryRemoveReadonlyAndArchiveAttribute(string fileName);

    /// <summary>
    /// Tries to remove the read-only and archive attributes from all files in the specified directory.
    /// </summary>
    /// <param name="directory">The path of the directory containing the files whose attributes are to be modified.</param>
    void TryRemoveReadonlyAndArchiveAttributesFromAll(string directory);
}