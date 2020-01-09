using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BookBuilder.Extensions;
using BookBuilder.Pipeline.Common;
using CommandLine;
using Markdig;
using Markdig.Renderers;
using Markdig.SyntaxHighlighting;

namespace BookBuilder
{
    static class Program
    {
        private static string mdExt = ".md";
        private static string targetExt = ".html";
        private static ProjectProcessing _processing;
        
        static void Main(string[] args)
        {
            _processing = new ProjectProcessing();
            var tasks = new ConcurrentQueue<Task>();
            
            // Parse and enqueue tasks
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    if (Directory.Exists(o.Path))
                    {
                        var po = new ProcessingOptions(o.Path, o.Output, true, targetExt);
                        tasks.Enqueue(Task.Run(() => ProcessFolderAsync(po)));
                    }
                    else
                    {
                        var po = new ProcessingOptions(o.Path, o.Output, false);
                        tasks.Enqueue(Task.Run(() => ProcessSingleMarkdownFileAsync(po)));    
                    }
                });
            
            // Wait for completion
            Task.WaitAll(tasks.ToArray());
        }

        private static async Task ProcessFolderAsync(ProcessingOptions opts)
        {
            var subfolders = Directory.GetDirectories(opts.SourcePath);
            foreach (var subfolder in subfolders)
            {
                var po = opts.Combine(subfolder);
                // Console.WriteLine($"Found local subfolder: ({po.SourcePath}) -> ({po.TargetPath})");
                await ProcessFolderAsync(po);
            }

            foreach (var filePath in Directory.GetFiles(opts.SourcePath))
            {
                if (Path.GetExtension(filePath.AsSpan()).Equals(mdExt.AsSpan(), StringComparison.Ordinal))
                {
                    await ProcessSingleMarkdownFileAsync(opts.Combine(filePath, true));
                }
                else
                {
                    await ProcessNonMarkdownFileAsync(opts.Combine(filePath, false));
                }
            }
        }

        private static async Task ProcessNonMarkdownFileAsync(ProcessingOptions opts)
        {
            var folderTargetPath = Path.GetDirectoryName(opts.TargetPath);
            if (!Directory.Exists(folderTargetPath)) Directory.CreateDirectory(folderTargetPath);

            await Task.Run(() =>
            {
                if(File.Exists(opts.TargetPath)) File.Delete(opts.TargetPath);
                File.Copy(opts.SourcePath, opts.TargetPath);
            });
        }

        private static async Task StartProcessing(ProcessingOptions opts)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseSyntaxHighlighting()
                .UseMarkdownLocalLinksPatchingExtension(opts)
                .UsePodcastFrameSupport(new PodcastSupportOptions{Width = "400px", Height = "102px"})
                .Build();

            var document = Markdown.Parse(File.ReadAllText(opts.SourcePath), pipeline);
            
            await using var writer = new StreamWriter(opts.TargetPath, false, Encoding.UTF8);

            var renderer = new HtmlRenderer(writer);
            pipeline.Setup(renderer);
            renderer.Writer = writer;
            renderer.Render(document);

            await renderer.Writer.FlushAsync();
        }

        private static async Task ProcessSingleMarkdownFileAsync(ProcessingOptions options)
        {
            if (!string.IsNullOrWhiteSpace(options.SourcePath) && File.Exists(options.SourcePath))
            {
                var folderTargetPath = Path.GetDirectoryName(options.TargetPath);
                if (!Directory.Exists(folderTargetPath)) Directory.CreateDirectory(folderTargetPath);
                try
                {
                    await StartProcessing(options);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine($"Given path isn't exists (\"{options.SourcePath}\")");
            }
        }
    }
}
