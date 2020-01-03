using System.IO;

namespace BookBuilder
{
    internal class ProcessingOptions
    {
        public ProcessingOptions(string src, string target, bool isFolder = true, string targetExt = null)
        {
            IsFolder = isFolder;
            TargetExt = targetExt;
            SourcePath = SourceRootPath = Path.IsPathRooted(src) ? src : Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), src));
            TargetPath = TargetRootPath = Path.IsPathRooted(target) ? target : Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), target));
        }

        private ProcessingOptions() { }

        public bool IsFolder { get; set; }

        public string TargetExt { get; private set; }

        public string SourcePath { get; private set; }
        
        public string SourceRootPath { get; private set; }
        
        public string TargetPath { get; private set; }
        
        public string TargetRootPath { get; private set; }

        public ProcessingOptions Combine(string subfolder, bool? isFile = null)
        {
            var relative_src = Path.GetFullPath(Path.Combine(SourcePath, subfolder));
            var relative_target = Path.GetFullPath(GetTargetByFullSource(relative_src));
            var isFolder = isFile.HasValue ? (bool)!isFile : IsFolder;
            
            if (!isFolder)
            {
                if (TargetExt != null && Path.HasExtension(relative_target))
                {
                    relative_target = Path.ChangeExtension(relative_target, TargetExt);
                }
            }

            return new ProcessingOptions
            {
                SourcePath = relative_src,
                SourceRootPath = SourceRootPath,
                TargetPath = relative_target,
                TargetRootPath = TargetRootPath,
                TargetExt = IsFolder ? TargetExt : string.Empty,
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