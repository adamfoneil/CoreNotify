using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoreNotify.Functions.Helpers;
using System;
using System.Collections.Generic;

namespace Testing
{
    [TestClass]
    public class UriBuilderExtensionTests
    {
        [TestMethod]
        public void WithDictionary()
        {
            var builder = new UriBuilder("https://whatever.com/this/that?greeting=hello");
            builder.AddQueryParameters(new Dictionary<string, object>()
            {
                ["date"] = new DateTime(2021, 4, 11),
                ["level"] = 1,
                ["key"] = "something"
            });
            
            Assert.IsTrue(builder.Uri.AbsoluteUri.Equals("https://whatever.com/this/that?greeting=hello&date=4/11/2021%2012:00:00%20AM&level=1&key=something"));
        }

        [TestMethod]
        public void WithObject()
        {
            var builder = new UriBuilder("https://whatever.com/this/that?greeting=hello");
            builder.AddQueryParameters(new
            {
                date = new DateTime(2021, 4, 11),
                level = 1,
                key = "something",
                greeting = "avast!"
            });
                                                        
            Assert.IsTrue(builder.Uri.AbsoluteUri.Equals("https://whatever.com/this/that?greeting=avast!&date=4/11/2021%2012:00:00%20AM&level=1&key=something"));
        }
    }
}
