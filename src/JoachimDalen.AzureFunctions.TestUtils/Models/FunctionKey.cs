namespace JoachimDalen.AzureFunctions.TestUtils.Models
{
    public class FunctionKey
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string FunctionName { get; set; }
        public FunctionAuthLevel Scope { get; set; }
    }
}