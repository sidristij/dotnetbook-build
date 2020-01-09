using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookBuilder.Pipeline;

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

        public void AddTask(IProcessingItem item)
        {
            if (item.MyStage < Stage)
            {
                throw new ArgumentException($"Cannot plan item to already done step: ${Stage}");
            }
            _queue[item.MyStage].Enqueue(item);
        }

        public async Task StartProcessingAsync()
        {
            var tasks = new List<Task>(128);
            foreach (var stage in ProcessingStageEx.Enumerate())
            {
                bool tasksGot;
                do
                {
                    tasksGot = false;
                    while (!_queue[stage].TryDequeue(out var item))
                    {
                        tasks.Add(item.DoWorkAsync());
                        tasksGot = true;
                    }
                    await Task.WhenAll(tasks.ToArray());
                    tasks.Clear();
                } while (tasksGot);
            }
        }
    }
}