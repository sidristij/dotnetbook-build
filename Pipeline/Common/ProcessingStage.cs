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
        public static IEnumerable<ProcessingStage> Enumerate()
        {
            return Enumerable
                .Range((int) ProcessingStage.Initial, (int) ProcessingStage.Finished - (int) ProcessingStage.Initial + 1)
                .Select(x => (ProcessingStage) x);
        }
    }
}