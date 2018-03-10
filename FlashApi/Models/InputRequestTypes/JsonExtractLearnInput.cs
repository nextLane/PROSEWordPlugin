namespace FlashApi.Models.InputRequestTypes
{
    public class JsonExtractLearnInput
    {
        public string text;

        public string type;
    }

    public enum JsonLearnType
    {
        None,
        NoJoin
    }
}