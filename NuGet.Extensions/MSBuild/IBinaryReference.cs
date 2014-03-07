namespace NuGet.Extensions.MSBuild
{
    public interface IBinaryReference {
        string IncludeVersion { get; }
        string IncludeName { get; }
        bool IsForAssembly(string assemblyFilename);
        bool TryGetHintPath(out string hintPath);

        /// <summary>
        /// Note: The parent project must be saved in order for this change to persist
        /// </summary>
        void SetHintPath(string value);
    }
}