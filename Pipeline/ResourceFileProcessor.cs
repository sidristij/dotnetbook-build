using System.IO;
using System.Threading.Tasks;
using BookBuilder.Pipeline.Common;

namespace BookBuilder.Pipeline
{
    internal class ResourceFileProcessor : IProcessingItem
    {
        protected ProjectProcessing ProjectProcessing { get; }
        protected ProcessingOptions Options { get; }

        public ResourceFileProcessor(ProjectProcessing projectProcessing, ProcessingOptions options)
        {
            ProjectProcessing = projectProcessing;
            Options = options;
        }
        
        public ProcessingStage MyStage => ProcessingStage.NonParsableFilesProcessing;
        
        public async Task DoWorkAsync()
        {
            var folderTargetPath = Path.GetDirectoryName(Options.TargetPath);
            if (!Directory.Exists(folderTargetPath)) Directory.CreateDirectory(folderTargetPath);

            await Task.Run(() =>
            {
                if(File.Exists(Options.TargetPath)) File.Delete(Options.TargetPath);
                File.Copy(Options.SourcePath, Options.TargetPath);
            });
        }
    }
}