using CoreNotify.Database;
using Dapper.QX.Abstract;
using Dapper.QX.Interfaces;
using System.Collections.Generic;

namespace CoreNotify.Service.Queries
{
    public class ValidateAccount : TestableQuery<Account>
    {
        public ValidateAccount() : base(
            @"SELECT 
                [a].*
            FROM 
                [dbo].[Account] [a]
                INNER JOIN [dbo].[AccountKey] [ak] ON [a].[Id]=[ak].[AccountId]
            WHERE 
                [a].[Name]=@name AND 
                [ak].[Key]=@key AND 
                [ak].[IsEnabled]=1")
        {
        }

        public string Name { get; set; }
        public string Key { get; set; }

        protected override IEnumerable<ITestableQuery> GetTestCasesInner()
        {
            yield return new ValidateAccount() { Name = "hello", Key = "whatever" };
        }
    }
}
