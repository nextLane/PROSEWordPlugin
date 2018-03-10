namespace FlashApi.Models.Processors
{
    using System.Collections.Generic;
    using System.Linq;

    using FlashApi.Models.InputRequestTypes;
    
    using Microsoft.ProgramSynthesis.Extraction.Text;
    using Microsoft.ProgramSynthesis.Extraction.Text.Constraints;
    using Microsoft.ProgramSynthesis.Extraction.Text.Semantics;

    public class TextExtractProcessor
    {
        public static bool IsSequence(List<TextExtractExample> textExtractExamples)
        {
            ////if there are more than one example defined for a string region, assuming to be a sequence
            var isSequence = false;

            foreach (var textExtractExample in textExtractExamples)
            {
                if (textExtractExample.selections.Count > 1)
                {
                    isSequence = true;
                }
            }

            return isSequence;
        }

        public string LearnSingle(List<TextExtractExample> textExtractExamples)
        {
            var session = new RegionSession();
            var regionExamples = new List<RegionExample>();

            foreach (var textExtractExample in textExtractExamples)
            {
                var inputRegion = RegionSession.CreateStringRegion(textExtractExample.text);
                var textExtractSelection = textExtractExample.selections.First(); // at most only one example is added per string region
                if (textExtractSelection != null)
                {
                    var exampleRegion = inputRegion.Slice((uint)textExtractSelection.startPos, (uint)textExtractSelection.endPos);
                    var regionExample = new RegionExample(inputRegion, exampleRegion);
                    regionExamples.Add(regionExample);
                }
            }

            session.AddConstraints(regionExamples);
            var program = session.Learn();

            return program.Serialize();
        }

        public string LearnSequence(List<TextExtractExample> textExtractExamples)
        {
            var session = new SequenceSession();
            var sequenceExamples = new List<SequenceExample>();

            foreach (var textExtractExample in textExtractExamples)
            {
                var inputRegion = SequenceSession.CreateStringRegion(textExtractExample.text);
                var exampleRegions = new List<StringRegion>();
                foreach (var textExtractSelection in textExtractExample.selections)
                {
                    var exampleRegion = inputRegion.Slice((uint)textExtractSelection.startPos, (uint)textExtractSelection.endPos);
                    exampleRegions.Add(exampleRegion);
                }

                sequenceExamples.Add(new SequenceExample(inputRegion, exampleRegions));
            }

            session.AddConstraints(sequenceExamples);
            var program = session.Learn();

            return program.Serialize();
        }

        public string RunSingle(string programAsString, string input)
        {
            var program = Loader.Instance.Region.Load(programAsString);
            var inputRegion = RegionSession.CreateStringRegion(input);

            var result = program.Run(inputRegion);
            return result.ToString();
        }

        public List<string> RunSequence(string programAsString, string input)
        {
            //programAsString = programAsString.Replace("\\", "");
            var program = Loader.Instance.Sequence.Load(programAsString);
            var inputRegion = SequenceSession.CreateStringRegion(input);

            var result = program.Run(inputRegion);

            return result.Select(region => region.Value).ToList();
        }
    }
}