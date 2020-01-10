using System.Threading.Tasks;

namespace BookBuilder.Pipeline.Common
{
    internal interface IProcessingItem
    {
        public ProcessingStage MyStage { get; }
        
        public bool ShouldWorkInExclusiveMode { get; }

        public Task DoWorkAsync();
    }
}