using System;
using System.Collections.Generic;
using System.Linq;
using BookBuilder.Pipeline.Common;

namespace BookBuilder.Pipeline.Templates
{
    internal class AggregateTemplateProcessor : TemplateBase
    {
        readonly List<ITemplate> _templatesProcessors;
        const int MaxCountAllowed = 100;
        
        public AggregateTemplateProcessor(Context context) : base(context)
        {
            _templatesProcessors = new List<ITemplate>
            {
                new DocumentBodyTemplate(context),
                new PathReplacementsTemplate(context),
                new LinkToTemplate(context),
                new ImportFileTemplate(context)
            };
        }

        /// <summary>
        /// Applies all templates processors while they do any work
        /// </summary>
        public override string Apply(string incoming)
        {
            string old;
            int count = 0;
            do
            {
                old = incoming;
                incoming = _templatesProcessors.Aggregate(incoming, (src, template) => template.Apply(src));
                count++;
            } while (incoming != old && count < MaxCountAllowed);

            if (count == MaxCountAllowed)
            {
                Console.WriteLine($"May be recursion in templates. Aborting work on file: {ProcessingOptions.SourcePath}");
            }
            
            return incoming;
        }
    }
}