namespace FlashApi.Models.InputRequestTypes
{
    using System.Collections.Generic;

    public class TextExtractLearnInput
    {
        public List<TextExtractExample> examples;
        
        public string type;
    }

    public class TextExtractExample
    {
        public List<TextExtractSelection> selections;

        public string text;
    }
    public class TextExtractSelection
    {
        public int startPos;

        public int endPos;
    }

    public enum TextExtractType
    {
        Sequence,
        Single
    }
}