using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Soenneker.Utils.FileSync.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;


namespace Soenneker.Utils.FileSync.Tests;

[Collection("Collection")]
public class FileUtilSyncTests : FixturedUnitTest
{
    private readonly IFileUtilSync _util;

    public FileUtilSyncTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<IFileUtilSync>(true);
    }

    [Fact]
    public void Default()
    {

    }
}
