namespace FlashApi.Controllers
{
    using System;
    using System.Web.Http;
    using System.Web.Http.Cors;

    using FlashApi.Models.InputRequestTypes;
    using FlashApi.Models.Processors;

    using Newtonsoft.Json;

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TextTransformController : ApiController
    {
        [HttpPost]
        public string Learn([FromBody] TextTransformLearnInput input)
        {
            try
            {
                var textProcessor = new TextTransformProcessor();
                var program = textProcessor.Learn(input.examples);
                return program;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        [HttpPost]
        public string Run([FromBody] TextTransformRunInput input)
        {
            var textProcessor = new TextTransformProcessor();
            var result = textProcessor.Run(input.program, input.text);
            return result;
        }
    }
}
