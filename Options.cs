using CommandLine;

namespace BookBuilder
{
    internal class Options
    {
        [Option('p', "path", Required = true, HelpText = "path to file to process")]
        public string Path { get; set; }

        [Option('r', "res", Required = true, HelpText = "path to resources source folder")]
        public string Resources { get; set; }

        [Option('w', "website", Required = false, HelpText = "Website relative path")]
        public string WebSiteRoot { get; set; }

        [Option('o', "output", Required = true, HelpText = "output path")]
        public string Output { get; set; }

        [Option('e', "error", Required = false, HelpText = "error output path if any")]
        public string ErrorLogs { get; set; }
    }
}