using System.Collections.Generic;
using System.Linq;

namespace BookBuilder.Pipeline.Common
{
    internal enum ProcessingStage
    {
        Initial,
        BeforeFoldersProcessing,
        FoldersProcessing,
        AfterFoldersProcessing,
        
        BeforeNonParsableFilesProcessing,
        NonParsableFilesProcessing,
        AfterNonParsableFilesProcessing,
        
        BeforeParsableFilesProcessing,
        ParsableFilesProcessing,
        AfterParsableFilesProcessing,
        
        BeforeParsing,
        Parsing,
        AfterParsing,
        
        BeforeRendering,
        Rendering,
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