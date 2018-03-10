namespace FlashApi.Models.InputRequestTypes
{
    using System.Collections.Generic;

    public class TextTransformLearnInput
    {
        public List<TextTransformExample> examples;
    }

    public class TextTransformExample
    {
        public string before;

        public string after;
    }
}