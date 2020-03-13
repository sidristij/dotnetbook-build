using System;
using System.IO;
using System.Threading.Tasks;
using BookBuilder.Pipeline;
using BookBuilder.Pipeline.Common;
using BookBuilder.Pipeline.Common.Structure;
using CommandLine;

namespace BookBuilder
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            var processing = new ProjectProcessing();
            var context = Context.Create().With(processing);

            // Parse and enqueue tasks
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    var templateFile = Path.Combine(Path.GetDirectoryName(o.Path), "template.htm");
                    string template = null;
                    if (File.Exists(templateFile))
                    {
                        template = File.ReadAllText(templateFile);
                    }
                    if (Directory.Exists(o.Path))
                    {
                        var po = new ProcessingOptions(o.Path, o.Output, true, ".html");
                        if (template != null)
                        {
                            context.With(new TemplateStorage(template));
                        }
                        processing.TryAddTask(new FolderProcessor(context.CreateCopy(po).With(new FolderDescription(null, null, "/"))));
                    }
                    else
                    {
                        var po = new ProcessingOptions(o.Path, o.Output, false);
                        processing.TryAddTask(new MarkdownFileProcessor(context.CreateCopy(po)));
                    }
                });

            await processing.StartProcessingAsync();
        }
    }

    public struct TemplateStorage
    {
        public TemplateStorage(string template)
        {
            Template = template;
        }
        public string Template { get; private set; }
    }

}
