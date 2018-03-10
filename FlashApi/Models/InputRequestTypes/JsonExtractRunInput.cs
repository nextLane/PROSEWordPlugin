namespace FlashApi.Models.InputRequestTypes
{
    public class JsonExtractRunInput
    {
        public string program;

        public string text;

        public string type;
    }

    public enum JsonRunType
    {
        None,
        InnerJoin,
        OuterJoin,
        Tree
    }
}