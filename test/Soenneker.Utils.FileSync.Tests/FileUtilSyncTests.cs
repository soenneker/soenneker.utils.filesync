using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Soenneker.Utils.FileSync.Abstract;
using Soenneker.Tests.HostedUnit;


namespace Soenneker.Utils.FileSync.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class FileUtilSyncTests : HostedUnitTest
{
    private readonly IFileUtilSync _util;

    public FileUtilSyncTests(Host host) : base(host)
    {
        _util = Resolve<IFileUtilSync>(true);
    }

    [Test]
    public void Default()
    {

    }
}
