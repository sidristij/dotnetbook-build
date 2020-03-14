namespace BookBuilder.Pipeline.Templates
{
    internal interface ITemplate
    {
        string Apply(string incoming);
    }
}