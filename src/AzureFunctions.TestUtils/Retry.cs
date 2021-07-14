using System;
using System.Threading.Tasks;

namespace AzureFunctions.TestUtils
{
    public static class Retry
    {
        public static async Task Try(Func<Task<bool>> action)
        {
            var maxCount = 10;
            while (true)
            {
                try
                {
                    var result = await action();
                    if (result) return;

                }
                catch (Exception e)
                {
                    if (--maxCount == 0)
                        throw;
                    await Task.Delay(500);
                }
            }
        }
    }
}