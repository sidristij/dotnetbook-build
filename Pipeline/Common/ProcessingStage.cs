using System.Collections.Generic;
using System.Linq;

namespace BookBuilder.Pipeline.Common
{
    internal enum ProcessingStage
    {
        Initial,
        BeforeProcessing,
        DocumentParsed,
        BeforeRenderingStarted,
        RenderingDone,
        Finished
    }
    
    internal static class ProcessingStageEx
    {
        public static IEnumerable<ProcessingStage> Enumerate()
        {
            return Enumerable
                .Range((int) ProcessingStage.Initial, (int) ProcessingStage.Finished - (int) ProcessingStage.Initial)
                .Select(x => (ProcessingStage) x);
        }
    }
}