using CoreNotify.Database;
using Dapper;
using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.Data.SqlClient;
using QueuedJobs.Library.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreNotify.Blazor.Server.Services
{
    public class QueuedJobRepository : JobRepositoryBase<QueuedJob, int>
    {
        private readonly string _connectionString;

        public const string UserName = "demo-user";

        public QueuedJobRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public override async Task<IEnumerable<QueuedJob>> ActiveJobsByUserAsync(string userName)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                return await cn.QueryAsync<QueuedJob>(
                   "SELECT * FROM [dbo].[QueuedJob] WHERE [UserName]=@userName AND [IsCleared]=0 ORDER BY [Created] ASC",
                   new { userName });
            }
        }

        public override async Task<QueuedJob> GetAsync(int key)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                return await cn.GetAsync<QueuedJob>(key);
            }
        }

        public override async Task<QueuedJob> SaveAsync(QueuedJob model)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                await cn.SaveAsync(model);
                return model;
            }
        }
    }
}
