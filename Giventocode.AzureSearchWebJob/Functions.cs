using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Giventocode.AzureSearchEntityManager;
using Microsoft.WindowsAzure;

namespace Giventocode.AzureSearchWebJob
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        
        public static async Task IndexEntity( [ServiceBusTrigger(IndexQueueManager.INDEXER_QUEUE)] string msg ,
             TextWriter log) 
         {
             var searchMan = new AzureSearchManager();              
             
             await searchMan.IndexDocumentAsync(msg);
         } 

    }
}
