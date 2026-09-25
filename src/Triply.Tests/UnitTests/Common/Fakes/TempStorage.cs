namespace Triply.Tests.UnitTests.Common.Fakes;

/// <summary>A throw-away folder that stands in for the shared uploads volume.</summary>
public sealed class TempStorage : IDisposable
{
    public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "triply-tests", 
        Guid.NewGuid().ToString());

    public string[] Files => Directory.Exists(Path) ? Directory.GetFiles(Path) : [];

    public void Dispose()
    {
        if (Directory.Exists(Path)) Directory.Delete(Path, recursive: true);
    }
}
