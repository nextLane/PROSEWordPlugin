namespace FlashApi.Models.Processors
{
    using Microsoft.ProgramSynthesis.Transformation.Json;
    using Microsoft.ProgramSynthesis.Wrangling.Constraints;

    using Newtonsoft.Json.Linq;

    public class JsonTransformProcessor
    {
        public string Learn(string trainInput, string trainOutput)
        {
            var traininputJToken = JToken.Parse(trainInput);
            var trainoutputJToken = JToken.Parse(trainOutput);

            var session = new Session();
            session.AddConstraints(new Example<JToken, JToken>(traininputJToken, trainoutputJToken));

            var topRankedProgram = session.Learn();
            if (topRankedProgram != null)
            {
                return topRankedProgram.Serialize();
            }

            return string.Empty;
        }

        public JToken Run(string programAsString, string input)
        {
            var program = Loader.Instance.Load(programAsString);
            var output = program.Run(input);
            return output;
        }
    }
}