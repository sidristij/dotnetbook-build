using System;
using System.IO;
using System.Threading.Tasks;
using BookBuilder.Pipeline.Common;
using BookBuilder.Pipeline.Common.Structure;

namespace BookBuilder.Pipeline
{
    internal class MarkdownFileProcessor : ProcessingItemBase
    {
        protected ProcessingOptions Options => Context.Get<ProcessingOptions>();
        protected FileDescription FileDescription => Context.Get<FileDescription>();

        public MarkdownFileProcessor(Context context) : base(context)
        {
        }

        public override ProcessingStage MyStage => ProcessingStage.ParsableFilesProcessing;
        
        public override async Task DoWorkAsync()
        {
            if (!string.IsNullOrWhiteSpace(Options.SourcePath) && File.Exists(Options.SourcePath))
            {
                var folderTargetPath = Path.GetDirectoryName(Options.TargetPath);
                if (!Directory.Exists(folderTargetPath)) Directory.CreateDirectory(folderTargetPath);

                var ctx = Context.CreateCopy();
                ProjectProcessing.TryAddTask(new MarkdownExtensionsProcessor(ctx));
            }
            else
            {
                Console.WriteLine($"Given path isn't exists (\"{Options.SourcePath}\")");
            }
        }
    }
}