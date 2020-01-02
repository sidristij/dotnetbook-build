using System;
using System.IO;
using System.Text;
using CommandLine;
using Markdig;
using Markdig.Renderers;
using Markdig.SyntaxHighlighting;

namespace bookbuild
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    if (!string.IsNullOrWhiteSpace(o.Path) && File.Exists(o.Path))
                    {
                        try
                        {
                            StartProcessing(o.Path, o.Output);
                        }
                        catch (Exception ex)
                        {
                            if (!string.IsNullOrEmpty(o.ErrorLogs))
                            {
                                File.WriteAllText(o.ErrorLogs, ex.Message);
                            }
                            else
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Given path isn't exists (\"{o.Path}\")");
                    }
                });
        }

        private static void StartProcessing(string inputPath, string outputPath)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseSyntaxHighlighting()
                .Build();

            using var writer = new StreamWriter(outputPath, false, Encoding.UTF8);

            pipeline.Setup(new HtmlRenderer(writer));

            var result = Markdown.ToHtml(File.ReadAllText(inputPath), pipeline);

            writer.Write(result);
        }
    }
}
