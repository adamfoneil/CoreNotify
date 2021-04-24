using CoreNotify.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CoreNotify.Extensions
{
    public static class FilterCollectionExtensions
    {
        public static void AddCoreNotifyValidation(this MvcOptions options, string key) => 
            options.Filters.Add(new CoreNotifyValidationFilter(key));        
    }
}
