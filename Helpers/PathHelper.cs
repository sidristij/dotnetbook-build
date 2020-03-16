using System;
using System.Text.RegularExpressions;

namespace BookBuilder.Helpers
{
    public static class PathHelper
    {
        private static readonly Regex pathSimplifyRegex = new Regex(@"[^\\/]+(?<!\.\.)[\\/]\.\.[\\/]");

        public static string Simplify(string path)
        {
            while (true)
            {
                var newPath = pathSimplifyRegex.Replace(path, "");
                if (newPath == path) break;
                path = newPath;
            }

            return path;
        }

        public static string MakeRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath)) throw new ArgumentNullException(nameof(fromPath));
            if (string.IsNullOrEmpty(toPath)) throw new ArgumentNullException(nameof(toPath));

            var fromUri = new Uri(fromPath);
            var toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            return relativePath;
        }

        public static string ReplaceSlashes(string path, bool web) =>
            web ? path.Replace('\\', '/') : path.Replace('/', '\\');
    }
}