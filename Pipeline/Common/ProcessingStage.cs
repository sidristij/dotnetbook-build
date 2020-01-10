using System.Collections.Generic;
using System.Linq;

namespace BookBuilder.Pipeline.Common
{
    internal enum ProcessingStage
    {
        Initial,
        FoldersProcessing,
        NonParsableFilesProcessing,
        ParsableFilesProcessing,
        BeforeParsing,
        AfterParsing,
        BeforeRendering,
        AfterRendering,
        Finished
    }
    internal static class ProcessingStageEx
    {
        private static readonly HashSet<ProcessingStage> Exclusives = new HashSet<ProcessingStage>
        {
            ProcessingStage.ParsableFilesProcessing
        };
        public static IEnumerable<(ProcessingStage Stage, bool Exclusive)> Enumerate()
        {
            return Enumerable
                .Range((int) ProcessingStage.Initial, (int) ProcessingStage.Finished - (int) ProcessingStage.Initial + 1)
                .Select(x => (Stage: (ProcessingStage) x, Exclusive: Exclusives.Contains((ProcessingStage)x)));
        }
    }
}