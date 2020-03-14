using BookBuilder.Pipeline.Common;

namespace BookBuilder.Pipeline.Templates
{
    internal class DocumentBodyTemplate : TemplateBase
    {
        private readonly DocumentHolder _documentHolder;

        public DocumentBodyTemplate(Context context) : base(context)
        {
            _documentHolder = context.Get<DocumentHolder>();
        }

        public override string Apply(string source)
        {
            return source.Replace("<!--BODY-->", _documentHolder.DocumentBody);
        }
    }
}