using System.Threading.Tasks;

namespace BookBuilder.Pipeline.Common
{
    internal abstract class ProcessingItemBase : IProcessingItem
    {
        protected ProcessingItemBase(Context context)
        {
            Context = context;
            ProjectProcessing = Context.Get<ProjectProcessing>();
        }

        protected ProjectProcessing ProjectProcessing { get; }
        
        protected Context Context { get; }

        public abstract ProcessingStage MyStage { get; }
        
        public virtual bool ShouldWorkInExclusiveMode => false;

        public abstract Task DoWorkAsync();
    }
}