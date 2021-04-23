using Microsoft.AspNetCore.Http;
using System.Linq;

namespace CoreNotify.Functions.Helpers
{
    public static class DbFunctionHelpers
    {
        public static int GetKey(HttpRequest request)
        {
            try
            {
                var id = request.Query["id"].First();
                return int.Parse(id);
            }
            catch
            {
                return 0;
            }
        }
    }
}
