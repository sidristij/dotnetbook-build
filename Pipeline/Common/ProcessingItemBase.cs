using System.Threading.Tasks;

namespace BookBuilder.Pipeline.Common
{
    internal abstract class ProcessingItemBase : IProcessingItem
    {
        protected ProcessingItemBase(ProjectProcessing projectProcessing)
        {
            ProjectProcessing = projectProcessing;
        }

        protected ProjectProcessing ProjectProcessing { get; }

        public abstract ProcessingStage MyStage { get; }
        public abstract Task DoWorkAsync();
    }
}