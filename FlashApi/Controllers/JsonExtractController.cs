namespace FlashApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Cors;

    using FlashApi.Models.InputRequestTypes;
    using FlashApi.Models.Processors;

    using Newtonsoft.Json;

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class JsonExtractController : ApiController
    {
        [HttpPost]
        public string Learn([FromBody] JsonExtractLearnInput input)
        {
            try
            {
                var jsonProcessor = new JsonExtractProcessor();
                if (input.type == JsonLearnType.NoJoin.ToString())
                {
                    return jsonProcessor.LearnWithNoJoin(input.text);
                }
                else
                {
                    return jsonProcessor.Learn(input.text);
                }
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        [HttpPost]
        public string Run([FromBody] JsonExtractRunInput input)
        {
            var jsonProcessor = new JsonExtractProcessor();
            if (input.type == JsonRunType.InnerJoin.ToString())
            {
                var result = jsonProcessor.RunWithInnerJoin(input.program, input.text);
                return JsonConvert.SerializeObject(result);
            }
            else if (input.type == JsonRunType.OuterJoin.ToString())
            {
                var result = jsonProcessor.RunWithOuterJoin(input.program, input.text);
                return JsonConvert.SerializeObject(result);
            }
            else if (input.type == JsonRunType.Tree.ToString())
            {
                var result = jsonProcessor.RunAsTree(input.program, input.text);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                var result = jsonProcessor.RunAsTable(input.program, input.text);
                return JsonConvert.SerializeObject(result);
            }
        }
    }
}