using System.Collections.Generic;
using System.Linq;
using BookBuilder.Pipeline.Common;

namespace BookBuilder.Pipeline.Templates
{
    internal class AggregateTemplateProcessor : TemplateBase
    {
        readonly List<ITemplate> _templatesProcessors;
        public AggregateTemplateProcessor(Context context) : base(context)
        {
            _templatesProcessors = new List<ITemplate>
            {
                new DocumentBodyTemplate(context),
                new PathReplacementsTemplate(context),
                new ImportFileTemplate(context)
            };
        }

        public override string Apply(string incoming)
        {
            string old;
            do
            {
                old = incoming;
                incoming = _templatesProcessors.Aggregate(incoming, (src, template) => template.Apply(src));
            } while (incoming != old);

            return incoming;
        }
    }
}