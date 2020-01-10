using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BookBuilder.Extensions;
using BookBuilder.Pipeline.Common;
using Markdig;
using Markdig.Renderers;
using Markdig.SyntaxHighlighting;

namespace BookBuilder.Pipeline
{
    internal class MarkdownFileProcessor : ProcessingItemBase
    {
        protected ProcessingOptions Options { get; }

        public MarkdownFileProcessor(Context context) : base(context)
        {
            Options = Context.Get<ProcessingOptions>();
        }

        public override ProcessingStage MyStage => ProcessingStage.ParsableFilesProcessing;
        public override async Task DoWorkAsync()
        {
            if (!string.IsNullOrWhiteSpace(Options.SourcePath) && File.Exists(Options.SourcePath))
            {
                var folderTargetPath = Path.GetDirectoryName(Options.TargetPath);
                if (!Directory.Exists(folderTargetPath)) Directory.CreateDirectory(folderTargetPath);
                try
                {
                    await StartProcessing();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine($"Given path isn't exists (\"{Options.SourcePath}\")");
            }
        }

        private async Task StartProcessing()
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseSyntaxHighlighting()
                .UseMarkdownLocalLinksPatchingExtension(Options)
                .UsePodcastFrameSupport(new PodcastSupportOptions{Width = "400px", Height = "102px"})
                .Build();

            var document = Markdown.Parse(File.ReadAllText(Options.SourcePath), pipeline);
            
            await using var writer = new StreamWriter(Options.TargetPath, false, Encoding.UTF8);

            var renderer = new HtmlRenderer(writer);
            pipeline.Setup(renderer);
            renderer.Writer = writer;
            renderer.Render(document);

            await renderer.Writer.FlushAsync();
        }
    }
}