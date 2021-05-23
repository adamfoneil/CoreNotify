using Azure.Storage.Queues;
using CoreNotify.Database;
using QueuedJobs.Abstract;

namespace CoreNotify.Blazor.Server.Services
{
    /// <summary>
    /// provides central access to all queues in the application that share a connection string
    /// </summary>
    public class QueueClientHelper : QueueClientHelperBase
    {
        public QueueClientHelper(string connectionString) : base(connectionString)
        {
        }

        public QueueClient ZipBuilder => this[QueuedJob.ZipBuilderQueue];
    }
}
