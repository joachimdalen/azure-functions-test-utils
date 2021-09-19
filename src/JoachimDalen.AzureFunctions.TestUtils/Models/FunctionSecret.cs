namespace JoachimDalen.AzureFunctions.TestUtils.Models
{
    public class FunctionSecret
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Encrypted { get; set; }
    }
}