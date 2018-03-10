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
    public class TextExtractController : ApiController
    {
        [HttpPost]
        public string Learn([FromBody] TextExtractLearnInput input)
        {
            var programLearned = string.Empty;
            var programType = string.Empty;

            try
            {
                var textProcessor = new TextExtractProcessor();
                if (input.type == TextExtractType.Single.ToString())
                {
                    programLearned = textProcessor.LearnSingle(input.examples);
                    programType = TextExtractType.Single.ToString();
                }
                else if (input.type == TextExtractType.Sequence.ToString())
                {
                    programLearned = textProcessor.LearnSequence(input.examples);
                    programType = TextExtractType.Sequence.ToString();
                }
                else
                {
                    if (TextExtractProcessor.IsSequence(input.examples))
                    {
                        programLearned = textProcessor.LearnSequence(input.examples);
                        programType = TextExtractType.Sequence.ToString();
                    }
                    else
                    {
                        programLearned = textProcessor.LearnSingle(input.examples);
                        programType = TextExtractType.Single.ToString();
                    }
                }

                var output = new TextExtractLearnOutput()
                {
                    program = programLearned,
                    type = programType
                };
                return JsonConvert.SerializeObject(output.program);
            }
            catch (Exception e)
            {
                return "Nothing learned";
            }
        }

        [HttpPost]
        public string Extract([FromBody] TextExtractLearnInput input) {
            var programLearned = string.Empty;
            var programType = string.Empty;
            var output = new TextExtractLearnAndRunOutput();
            var results = "No results";

            try {
                var textProcessor = new TextExtractProcessor();
                if (input.type == TextExtractType.Single.ToString()) {
                    programLearned = textProcessor.LearnSingle(input.examples);
                    programType = TextExtractType.Single.ToString();
                }
                else if (input.type == TextExtractType.Sequence.ToString()) {
                    programLearned = textProcessor.LearnSequence(input.examples);
                    programType = TextExtractType.Sequence.ToString();
                }
                else {
                    if (TextExtractProcessor.IsSequence(input.examples)) {
                        programLearned = textProcessor.LearnSequence(input.examples);
                        programType = TextExtractType.Sequence.ToString();
                    }
                    else {
                        programLearned = textProcessor.LearnSingle(input.examples);
                        programType = TextExtractType.Single.ToString();
                    }
                }

                var serializedProg = programLearned;
                
                if(!string.IsNullOrEmpty(programLearned)) {
                    var inp = new TextExtractRunInput();
                    inp.program = programLearned;
                    inp.text = input.examples[0].text;
                    inp.type = TextExtractType.Sequence.ToString();
                    results = Run(inp);
                }
            }
            catch (Exception e) {
                return JsonConvert.SerializeObject(results
                );
            }

            return JsonConvert.SerializeObject(
                results
            );
        }

        [HttpPost]
        public string Run([FromBody] TextExtractRunInput input)
        {
            try
            {
                var textProcessor = new TextExtractProcessor();
                if (input.type == TextExtractType.Sequence.ToString())
                {
                    var output = textProcessor.RunSequence(input.program, input.text);
                    return JsonConvert.SerializeObject(output);
                }
                else
                {
                    var output = textProcessor.RunSingle(input.program, input.text);
                    return output;
                }
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
    }
}