namespace FlashApi.Controllers
{
    using System;
    using System.Web.Http;
    using System.Web.Http.Cors;

    using FlashApi.Models.InputRequestTypes;
    using FlashApi.Models.Processors;

    using Newtonsoft.Json;

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class JsonTransformController : ApiController
    {
        [HttpPost]
        public string Learn([FromBody] JsonTransformLearnInput input)
        {
            try
            {
                var jsonProcessor = new JsonTransformProcessor();
                var program = jsonProcessor.Learn(input.trainInput, input.trainOutput);
                return program;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        [HttpPost]
        public string Run([FromBody] JsonTransformRunInput input)
        {
            try
            {
                var jsonProcessor = new JsonTransformProcessor();
                var result = jsonProcessor.Run(input.program, input.text);
                return result.ToString();
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
    }
}
