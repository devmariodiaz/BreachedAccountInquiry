using Inquiry.Func.Abstractions;
using Inquiry.Func.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Inquiry.Func.Functions
{
    [StorageAccount("AzureWebJobsStorage")]
    public class PersistBreachedInfoFun
    {
        private readonly IBreachInquiryService _breachServices;

        public PersistBreachedInfoFun(IBreachInquiryService breachServices)
        {
            _breachServices = breachServices ?? throw new ArgumentNullException(nameof(breachServices));
        }

        [FunctionName("ProcessRequests")]
        public async Task Run(
            [QueueTrigger("azuretestappqueue")] BreachInquiryRequest myQueueItem,
            [CosmosDB(
            databaseName: "azure-test-DB",
            collectionName: "BreachedAccounts",
            ConnectionStringSetting = "AzureCosmosDBConnectionString")] IAsyncCollector<BreachedInfo> breachedInfoCosmosDB,
            ILogger _log
           )
        {
            _log.LogInformation($"Processing breached account requests for account: {myQueueItem.Account} is starting.");

            if (myQueueItem?.Account.Trim().Length == 0)
                throw new ArgumentNullException($"{nameof(myQueueItem.Account)} must have a value!");

            var breachedInfo = await _breachServices.GetBrechedInfoAsync(myQueueItem.Account);

            if (breachedInfo?.Name.Trim().Length > 0)
            {
                await breachedInfoCosmosDB.AddAsync(breachedInfo);
                _log.LogInformation($"Breached account info for account:{myQueueItem.Account} persisted.");
            }


        }
    }
}
