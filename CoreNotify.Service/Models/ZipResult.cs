using System;

namespace CoreNotify.Service.Models
{
    public class ZipResult
    {
        /// <summary>
        /// public URL for accessing zip file
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// when does link stop working?
        /// </summary>
        public DateTime ExpiresAfter { get; set; }
        /// <summary>
        /// for direct blob access by downstream processes
        /// </summary>
        public string BlobName { get; set; }
    }
}
