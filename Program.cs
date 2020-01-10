using System.IO;
using System.Threading.Tasks;
using BookBuilder.Pipeline;
using BookBuilder.Pipeline.Common;
using CommandLine;

namespace BookBuilder
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            var processing = new ProjectProcessing();
            
            // Parse and enqueue tasks
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    if (Directory.Exists(o.Path))
                    {
                        var po = new ProcessingOptions(o.Path, o.Output, true, ".html");
                        processing.TryAddTask(new FolderProcessor(processing, po));
                    }
                    else
                    {
                        var po = new ProcessingOptions(o.Path, o.Output, false);
                        processing.TryAddTask(new MarkdownFileProcessor(processing, po));
                    }
                });
            
            await processing.StartProcessingAsync();
        }
    }
}
