[![](https://img.shields.io/nuget/v/Soenneker.Utils.FileSync.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Utils.FileSync/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.filesync/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.utils.filesync/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Utils.FileSync.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Utils.FileSync/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.filesync/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.utils.filesync/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Utils.FileSync
A utility library encapsulating synchronous file IO operations.

## Installation

```bash
dotnet add package Soenneker.Utils.FileSync
```

## Quick start

```csharp
using Soenneker.Utils.FileSync.Registrars;

services.AddFileUtilSyncAsSingleton();
```

Then inject `IFileUtilSync` wherever you need it.

## Common operations

- `GetTempFileName()` - Returns a unique GUID path under the system temp directory without creating the file.
- `Read()` - Reads all text from the specified file. Returns the file contents as a string.
- `ReadToBytes()` - Reads all bytes from the specified file. Returns the file contents as a byte array.
- `ReadAsLines()` - Reads all lines from the specified file. Returns a list of lines from the file.
- `WriteAllLines()` - Writes the specified lines to a file.
- `Write()` - Writes text content to a file.
- `Exists()` - Determines whether the specified file exists.
- `Delete()` - Deletes the specified file.
- `DeleteIfExists()` - Deletes the file if it exists. Returns true if the file was deleted; otherwise, false.
- `TryDeleteIfExists()` - Tries to delete the file if it exists, catching any exceptions. Returns true if the file was deleted; otherwise, false.
- `TryRemoveReadonlyAndArchiveAttributesFromAll()` - Removes read-only and archive attributes from all files in a directory.
- `TryRemoveReadonlyAndArchiveAttribute()` - Removes read-only and archive attributes from a single file. Returns true if attributes were removed; otherwise, false.

The package also includes 12 additional operations for more specialized cases.
