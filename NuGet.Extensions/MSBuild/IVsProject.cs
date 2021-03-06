using System.Collections.Generic;
using System.IO;

namespace NuGet.Extensions.MSBuild
{
    public interface IVsProject {
        IEnumerable<IReference> GetBinaryReferences();
        string AssemblyName { get; }
        string ProjectName { get; }
        DirectoryInfo ProjectDirectory { get; }
        void Save();
        void AddFile(string filename);
        IEnumerable<IReference> GetProjectReferences();
    }
}