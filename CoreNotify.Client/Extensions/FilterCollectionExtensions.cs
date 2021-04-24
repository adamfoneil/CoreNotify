using CoreNotify.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CoreNotify.Extensions
{
    public static class FilterCollectionExtensions
    {
        public static void AddCoreNotifyAuthorization(this MvcOptions options, string key) => 
            options.Filters.Add(new CoreNotifyAuthorizationFilter(key));        
    }
}
