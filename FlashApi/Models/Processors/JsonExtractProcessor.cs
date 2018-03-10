namespace FlashApi.Models.Processors
{
    using System.Collections.Generic;

    using Microsoft.ProgramSynthesis.Extraction.Json;
    using Microsoft.ProgramSynthesis.Extraction.Json.Constraints;
    using Microsoft.ProgramSynthesis.Wrangling.Json;
    using Microsoft.ProgramSynthesis.Wrangling.Schema;
    using Microsoft.ProgramSynthesis.Wrangling.Schema.TableOutput;
    using Microsoft.ProgramSynthesis.Wrangling.Schema.TreeOutput;

    public class JsonExtractProcessor
    {
        public string Learn(string input)
        {
            var session = new Session();
            session.AddConstraints(new FlattenDocument(input));
            var program = session.Learn();
            
            return program.Serialize();
        }

        public string LearnWithNoJoin(string input)
        {
            var noJoinSession = new Session();
            noJoinSession.AddConstraints(new FlattenDocument(input), new NoJoinInnerArrays());
            var noJoinProgram = noJoinSession.Learn();

            return noJoinProgram.Serialize();
        }

        public IEnumerable<TableRow<JsonRegion>> RunAsTable(string programAsString, string input)
        {
            var program = Loader.Instance.Load(programAsString);
            return program.RunTable(input);
        }

        public ITreeOutput<JsonRegion> RunAsTree(string programAsString, string input)
        {
            var program = Loader.Instance.Load(programAsString);
            var tree = program.Run(input);
            return tree;
        }

        public IEnumerable<TableRow<JsonRegion>> RunWithOuterJoin(string programAsString, string input)
        {
            var program = Loader.Instance.Load(programAsString);
            return program.RunTable(input, TreeToTableSemantics.OuterJoin);
        }

        public IEnumerable<TableRow<JsonRegion>> RunWithInnerJoin(string programAsString, string input)
        {
            var program = Loader.Instance.Load(programAsString);
            return program.RunTable(input, TreeToTableSemantics.InnerJoin);
        }
    }
}