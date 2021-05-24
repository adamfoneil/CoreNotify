namespace CoreNotify.Service.Models
{
    /// <summary>
    /// a request to zip all blobs at a given path
    /// </summary>
    public class ZipRequest
    {
        public string ContainerName { get; set; }
        public string BlobPrefix { get; set; }
    }
}
