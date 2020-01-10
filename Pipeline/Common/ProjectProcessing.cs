using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BookBuilder.Pipeline.Common
{
    internal class ProjectProcessing
    {
        private Dictionary<ProcessingStage, ConcurrentQueue<IProcessingItem>> _queue;

        private ProcessingStage Stage { get; set; }

        public ProjectProcessing()
        {
            _queue = new Dictionary<ProcessingStage, ConcurrentQueue<IProcessingItem>>();
            foreach (var processingStage in ProcessingStageEx.Enumerate())
            {
                _queue[processingStage] = new ConcurrentQueue<IProcessingItem>();
            }
        }

        public bool TryAddTask(IProcessingItem item)
        {
            if (item.MyStage < Stage)
            {
                return false;
            }
            _queue[item.MyStage].Enqueue(item);
            return true;
        }

        public async Task StartProcessingAsync()
        {
            var tasks = new List<Task>(128);
            var pair = new ConcurrentExclusiveSchedulerPair(TaskScheduler.Current);
            foreach (var stage in ProcessingStageEx.Enumerate())
            {
                Stage = stage;
                bool tasksGot;
                do
                {
                    tasksGot = false;
                    while (_queue[stage].TryDequeue(out var item))
                    {
                        var task = item;
                        var onStage = stage;
                        tasks.Add(Task.Factory.StartNew(async () =>
                        {
                            try
                            {
                                await task.DoWorkAsync();
                            }
                            catch(Exception ex)
                            {
                                Console.WriteLine($"Error while processing {onStage} stage: {ex.Message}");
                            }
                        }, 
                        CancellationToken.None, 
                        TaskCreationOptions.None, 
                        task.ShouldWorkInExclusiveMode ? pair.ExclusiveScheduler : pair.ConcurrentScheduler));
                        
                        tasksGot = true;
                    }
                    await Task.WhenAll(tasks.ToArray());
                    tasks.Clear();
                } while (tasksGot);
            }
        }
    }
}