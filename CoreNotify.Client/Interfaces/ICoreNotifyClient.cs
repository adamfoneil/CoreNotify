using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreNotify.Interfaces
{
    internal interface ICoreNotifyClient
    {
        Task LoginAsync(string userName, string password);

        Task RegisterAsync(string userName, string password, string accountName);

        
    }
}
