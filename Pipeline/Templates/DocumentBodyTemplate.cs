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

        /// <summary>
        /// Global template, which is used to generate the whole file
        /// </summary>
        public override string Apply(string source)
        {
            return source.Replace("<!--document-body-->", _documentHolder.DocumentBody);
        }
    }
}