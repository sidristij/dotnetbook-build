using System.IO;
using System.Threading.Tasks;
using BookBuilder.Pipeline.Common;

namespace BookBuilder.Pipeline
{
    internal class ResourceFileProcessor : ProcessingItemBase
    {
        protected ProcessingOptions Options => Context.Get<ProcessingOptions>();

        public ResourceFileProcessor(Context context) : base(context)
        {
        }
        
        public override ProcessingStage MyStage => ProcessingStage.NonParsableFilesProcessing;
        
        public override async Task DoWorkAsync()
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