﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NuGet.Common;

namespace NuGet.Extensions.Repositories
{
    /// <summary>
    /// Provides the ability to search across IQueryable package sources for a set of packages that contain a particular assembly or set of assemblies.
    /// </summary>
    public class RepositoryAssemblyResolver
    {
        readonly List<string> _assemblies = new List<string>();
        readonly IQueryable<IPackage> _packageSource;
        private readonly IFileSystem _fileSystem;
        private readonly IConsole _console;
        private readonly Dictionary<string, List<IPackage>> _resolvedAssemblies;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryAssemblyResolver"/> class.
        /// </summary>
        /// <param name="assemblies">The assemblies to look for.</param>
        /// <param name="packageSource">The package sources to search.</param>
        /// <param name="fileSystem">The file system to output any packages.config files.</param>
        /// <param name="console">The console to output to.</param>
        public RepositoryAssemblyResolver(List<string> assemblies, IQueryable<IPackage> packageSource, IFileSystem fileSystem, IConsole console)
        {
            _packageSource = packageSource;
            _fileSystem = fileSystem;
            _console = console;

            var lookup = assemblies.ToLookup(a => a, a => "");
            foreach (var assembly in lookup.Where(a => a.Count() > 1)) console.WriteWarning("Same assembly resolution will be used for both assembly references for {0}", assembly);
            _assemblies = assemblies.Distinct().ToList();
            _resolvedAssemblies = _assemblies.ToDictionary(a => a, _ => new List<IPackage>());
        }
        
        /// <summary>
        /// Resolves a list of packages that contain the assemblies requested.
        /// </summary>
        /// <param name="exhaustive">if set to <c>true</c> [exhaustive].</param>
        /// <returns></returns>
        public AssemblyToPackageMapping GetAssemblyToPackageMapping(Boolean exhaustive)
        {
            int current = 0;
            int max = _packageSource.Count();

            foreach (var package in _packageSource)
            {
                _console.WriteLine("Checking package {0} of {1}", current++, max);
                var packageFiles = package.GetFiles();
                foreach (var f in packageFiles)
                {
                    var file = new FileInfo(f.Path);
                    foreach (var assembly in _assemblies.Where(a => a.Equals(file.Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        _resolvedAssemblies[assembly].Add(package);
                        //HACK Exhaustive not easy with multiple assemblies, so default to only one currently....
                        if (!exhaustive && _assemblies.Count == 1)
                        {
                            return new AssemblyToPackageMapping(_console, _fileSystem, _resolvedAssemblies);
                        }
                    }
                }
            }
            return new AssemblyToPackageMapping(_console, _fileSystem, _resolvedAssemblies);
        }
    }


}
