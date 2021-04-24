using System;
using System.Collections.Generic;
using System.Text;

namespace CoreNotify.Attributes
{
    /// <summary>
    /// indicates that an MVC action must have a CoreNotify key in the request header
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CoreNotifyAuthorizeAttribute : Attribute
    {
    }
}
