using Newtonsoft.Json;

namespace JoachimDalen.AzureFunctions.TestUtils.Models
{
    public class FunctionSecretRoot
    {
        public FunctionSecretRoot()
        {
            HostName = "localhost:7071";
            InstanceId = "some-instance";
            Source = "runtime";
            DecryptionKeyId = "";
        }

        [JsonProperty("masterKey", NullValueHandling = NullValueHandling.Ignore)]
        public FunctionSecret MasterKey { get; set; }

        [JsonProperty("functionKeys", NullValueHandling = NullValueHandling.Ignore)]
        public FunctionSecret[] FunctionKeys { get; set; }

        [JsonProperty("systemKeys", NullValueHandling = NullValueHandling.Ignore)]
        public FunctionSecret[] SystemKeys { get; set; }

        [JsonProperty("keys", NullValueHandling = NullValueHandling.Ignore)]
        public FunctionSecret[] Keys { get; set; }

        [JsonProperty("hostName")]
        public string HostName { get; set; }

        [JsonProperty("instanceId")]
        public string InstanceId { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("decryptionKeyId")]
        public string DecryptionKeyId { get; set; }
    }
}