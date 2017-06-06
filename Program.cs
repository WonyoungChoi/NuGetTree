using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Configuration;
using NuGet.Versioning;

namespace NuGetTree
{
    class Program
    {
      Logger _logger;
      SourceRepository _repository;

      public Program() {
        _logger = new Logger();
        List<Lazy<INuGetResourceProvider>> providers = new List<Lazy<INuGetResourceProvider>>();
        providers.AddRange(Repository.Provider.GetCoreV3());  // Add v3 API support
        PackageSource packageSource = new PackageSource("https://tizen.myget.org/F/dotnet/api/v3/index.json");
        _repository = new SourceRepository(packageSource, providers);

      }

      public async Task GetPackageInfo(string packageId, string packageVersion) {
        PackageMetadataResource packageMetadataResource = await _repository.GetResourceAsync<PackageMetadataResource>();
        PackageIdentity id = new PackageIdentity(packageId, new NuGetVersion(packageVersion));
        IPackageSearchMetadata data = await packageMetadataResource.GetMetadataAsync(id, _logger, CancellationToken.None);
        Console.WriteLine("{0} {1}", data.Identity.Id, data.Identity.Version);
        IEnumerable<PackageDependencyGroup> dependencyGroups = data.DependencySets;
        foreach (var group in dependencyGroups) {
          Console.WriteLine("  {0}:", group.TargetFramework);
          foreach (PackageDependency package in group.Packages) {
            Console.WriteLine("    {0} {1}", package.Id, package.VersionRange);
          }
        }
      }

      static void Main(string[] args)
      {
        Program p = new Program();
        p.GetPackageInfo("Tizen.NET", "3.0.0-test1").Wait();
      }
    }

    public class Logger : ILogger
    {
      public void LogDebug(string data) => Console.WriteLine("DEBUG: {0}", data);
      public void LogVerbose(string data) => Console.WriteLine("VERBOSE: {0}", data);
      public void LogInformation(string data) => Console.WriteLine("INFO: {0}", data);
      public void LogMinimal(string data) => Console.WriteLine("MINIMAL: {0}", data);
      public void LogWarning(string data) => Console.WriteLine("WARN: {0}", data);
      public void LogError(string data) => Console.WriteLine("ERROR: {0}", data);
      public void LogErrorSummary(string data) => Console.WriteLine("SUMMARY: {0}", data);
      public void LogInformationSummary(string data) => Console.WriteLine("SUMMARY: {0}", data);
    }
}
