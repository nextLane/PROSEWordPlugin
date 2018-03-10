namespace FlashApi.Models.Processors
{
    using System.Collections.Generic;

    using FlashApi.Models.InputRequestTypes;

    using Microsoft.ProgramSynthesis.Transformation.Text;
    
    public class TextTransformProcessor
    {
        public string Learn(List<TextTransformExample> textTransformExamples)
        {
            var session = new Session();
            var examples = new List<Example>();
            foreach (var textTransformExample in textTransformExamples)
            {
                var example = new Example(new InputRow(textTransformExample.before), textTransformExample.after);
                examples.Add(example);
            }

            session.AddConstraints(examples);
            var program = session.Learn();
            return program.Serialize();
        }

        public string Run(string serializedProgram, string input)
        {
            var program = Loader.Instance.Load(serializedProgram);
            var result = program.Run(new InputRow(input)) as string;
            return result;
        }
    }
}