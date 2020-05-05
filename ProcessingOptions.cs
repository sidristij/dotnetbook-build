using System.IO;

namespace BookBuilder
{
    internal class ProcessingOptions
    {
        public ProcessingOptions(string src, string target, string resourcesSource, bool isFolder = true, string targetExt = null)
        {
            ResourcesSource = resourcesSource;
            IsFolder = isFolder;
            TargetExt = targetExt;
            SourcePath = SourceRootPath = Path.IsPathRooted(src) ? src : Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), src));
            TargetPath = TargetRootPath = Path.IsPathRooted(target) ? target : Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), target));
        }

        private ProcessingOptions() { }

        public string ResourcesSource { get; set; }

        public bool IsFolder { get; set; }

        public string TargetExt { get; private set; }

        public string SourcePath { get; private set; }
        
        public string SourceRootPath { get; private set; }
        
        public string TargetPath { get; private set; }
        
        public string TargetRootPath { get; private set; }

        public ProcessingOptions Combine(string subfolder, bool? isFile = null)
        {
            var relativeSrc = Path.GetFullPath(Path.Combine(SourcePath, subfolder));
            var relativeTarget = Path.GetFullPath(GetTargetByFullSource(relativeSrc));
            var isFolder = isFile.HasValue ? (bool)!isFile : IsFolder;
            
            if (!isFolder)
            {
                if (TargetExt != null && Path.HasExtension(relativeTarget))
                {
                    relativeTarget = Path.ChangeExtension(relativeTarget, TargetExt);
                }
            }

            return new ProcessingOptions
            {
                ResourcesSource = ResourcesSource,
                SourcePath = relativeSrc,
                SourceRootPath = SourceRootPath,
                TargetPath = relativeTarget,
                TargetRootPath = TargetRootPath,
                TargetExt = TargetExt,
                IsFolder = isFolder
            };
        }

        private string GetTargetByFullSource(string relativeSrc)
        {
            var relToSrcCombined = Path.GetRelativePath(SourceRootPath, relativeSrc);
            return GetTargetByRelativeSource(relToSrcCombined);
        }
        
        private string GetTargetByRelativeSource(string relativeSrc)
        {
            return Path.Combine(TargetRootPath, relativeSrc);
        }
    }
}