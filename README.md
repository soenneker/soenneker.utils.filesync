[![](https://img.shields.io/nuget/v/Soenneker.Utils.FileSync.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Utils.FileSync/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.filesync/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.utils.filesync/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Utils.FileSync.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Utils.FileSync/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.filesync/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.utils.filesync/actions/workflows/codeql.yml)

# Soenneker.Utils.FileSync

DI-friendly synchronous file reading, writing, copying, moving, enumeration, attribute cleanup, and deletion.

## Installation

```bash
dotnet add package Soenneker.Utils.FileSync
```

## Registration

```csharp
builder.Services.AddFileUtilSyncAsSingleton();
```

Scoped registration is also available with `AddFileUtilSyncAsScoped()`.

All operations block the calling thread. Use this package for genuinely synchronous workflows; use `Soenneker.Utils.File` on request, UI, or other threads where blocking I/O harms responsiveness.

## Read and write

```csharp
string text = files.Read(path);
byte[] bytes = files.ReadToBytes(path);
List<string> lines = files.ReadAsLines(path);

files.Write(path, text);
files.WriteAllLines(path, lines);
```

Whole-file operations materialize the complete contents. The stream overload rewinds the supplied stream to position zero, copies it, and leaves the source open; it therefore requires a seekable source.

`FileUtilSync.GetTempFileName()` returns a unique candidate path under the system temporary directory. It does not create or reserve the file, so another actor can claim the path before it is used.

## Copy and move

```csharp
files.Copy(sourceFile, destinationFile);
files.Move(sourceFile, destinationFile);

files.CopyDirectory(sourceDirectory, destinationDirectory, overwrite: true);
files.CopyRecursively(sourceDirectory, destinationDirectory, overwrite: true);
```

Single-file `Copy()` and `Move()` do not overwrite an existing destination or create its parent directory. `CopyDirectory()` creates the destination but copies only top-level files. `CopyRecursively()` creates the directory tree and copies descendants while skipping symbolic links, junctions, and other reparse points.

Copy operations are incremental rather than transactional. If a later file fails, earlier destination files remain copied.

## Deletion and bulk changes

```csharp
bool deleted = files.DeleteIfExists(path);
bool attempted = files.TryDelete(path);

files.DeleteAll(rootDirectory);
files.TryRemoveReadonlyAndArchiveAttributesFromAll(rootDirectory);
```

`DeleteAll()` recursively deletes discovered files but leaves directories in place. Its `Try*` counterpart processes files best-effort and logs failures. Recursive safe enumeration skips inaccessible paths and reparse points.

`RenameAllInDirectoryRecursively()` performs a literal, case-sensitive replacement over each full file path and moves files one at a time. It does not rename empty directories, create missing destination parents, detect all conflicts in advance, or roll back earlier moves. Use it only with a validated root and review broad renames in version control.
