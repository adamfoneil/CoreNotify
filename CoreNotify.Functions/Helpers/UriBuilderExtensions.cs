using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreNotify.Functions.Helpers
{
    public static class UriBuilderExtensions
    {
        public static void AddQueryParameters(this UriBuilder uriBuilder, IDictionary<string, object> parameters)
        {
            const char paramSeparator = '&';
            const char keyValueSepartor = '=';

            var queryParams = (!string.IsNullOrEmpty(uriBuilder.Query)) ? 
                uriBuilder.Query.Split(paramSeparator).Select(item =>
                {
                    var parts = item.Split(keyValueSepartor);
                    var key = (parts[0].StartsWith("?")) ? parts[0].Substring(1) : parts[0];
                    return new KeyValuePair<string, object>(key, parts[1]);
                }).ToDictionary(kp => kp.Key, kp => kp.Value) :
                new Dictionary<string, object>();

            foreach (var kp in parameters)
            {
                queryParams[kp.Key] = kp.Value;
            }

            uriBuilder.Query = string.Join(paramSeparator, queryParams.Select(kp => string.Join(keyValueSepartor, new string[]
            {
                kp.Key, kp.Value.ToString()
            })));
        }

        public static void AddQueryParameter(this UriBuilder uriBuilder, string name, object value) => 
            AddQueryParameters(uriBuilder, new Dictionary<string, object>()
            {
                [name] = value
            });

        public static void AddQueryParameters(this UriBuilder uriBuilder, object parameters)
        {
            var props = parameters.GetType().GetProperties().Where(pi => pi.CanRead).ToArray();
            var dictionary = props.ToDictionary(pi => pi.Name, pi => pi.GetValue(parameters));
            AddQueryParameters(uriBuilder, dictionary);
        }
    }
}
