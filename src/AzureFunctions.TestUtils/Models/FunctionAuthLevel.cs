namespace AzureFunctions.TestUtils.Models
{
    public enum FunctionAuthLevel
    {
        Anonymous = 0,
        User = 1,
        Function = 2,
        System = 3,
        Admin = 4,
    }
}