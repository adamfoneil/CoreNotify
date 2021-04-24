using CoreNotify.Attributes;
using CoreNotify.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace CoreNotify.Filters
{
    public class CoreNotifyAuthorizationFilter : IAuthorizationFilter
    {
        private readonly string _validationKey;

        public CoreNotifyAuthorizationFilter(string validationKey)
        {
            _validationKey = validationKey;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                var authAttr = context.ActionDescriptor.EndpointMetadata.OfType<CoreNotifyAuthorizeAttribute>().SingleOrDefault();
                if (authAttr != null)
                {
                    var key = context.HttpContext.Request.Query[Notification.QueryStringKey];
                    if (key.Equals(_validationKey)) return;
                    context.Result = new UnauthorizedResult();
                }                                              
            }
            catch 
            {
                // do nothing
            }            
        }
    }
}
