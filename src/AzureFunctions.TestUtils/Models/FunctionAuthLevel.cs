namespace AzureFunctions.TestUtils.Models
{
    public enum FunctionAuthLevel
    {
        /// <summary>
        /// Allow access to requests that include a function key
        /// </summary>
        Function = 2,

        /// <summary>
        /// Allows access to requests that include a system key
        /// </summary>
        System = 3,

        /// <summary>
        /// Allow access to requests that include the master key
        /// </summary>
        Admin = 4,
    }
}