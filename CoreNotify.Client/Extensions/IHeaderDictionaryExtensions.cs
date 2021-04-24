using CoreNotify.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace CoreNotify.Extensions
{
    public static class IHeaderDictionaryExtensions
    {
        public static void AddSubjectLine(this IHeaderDictionary headers, string subject)
        {
            headers.Add(new KeyValuePair<string, StringValues>(Notification.SubjectHeader, new StringValues(subject)));
        }
    }
}
