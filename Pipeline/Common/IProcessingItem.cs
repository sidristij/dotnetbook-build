using System.Threading.Tasks;

namespace BookBuilder.Pipeline.Common
{
    internal interface IProcessingItem
    {
        public ProcessingStage MyStage { get; }

        public Task DoWorkAsync();
    }
}