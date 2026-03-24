using Markdig;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Syntax;

/// <summary>
/// Handles lists, splitting them into two when lists have gap between them
/// </summary>
public class StrictListExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        var listParser = pipeline.BlockParsers.Find<ListBlockParser>();
        
        if (listParser != null)
            pipeline.BlockParsers.Remove(listParser);

        //needs to be before heading list parser
        pipeline.BlockParsers.Insert(0, new StrictListParser());
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) { }
}

public class StrictListParser : ListBlockParser
{
    public override BlockState TryContinue(BlockProcessor processor, Block block)
    {
        if (processor.IsBlankLine)
            return BlockState.Break;
    
        return base.TryContinue(processor, block);
    }
}