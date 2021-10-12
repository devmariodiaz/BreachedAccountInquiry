using Inquiry.Func.Extensions;
using Inquiry.Func.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Inquiry.Func.Functions
{
    [StorageAccount("AzureWebJobsStorage")]
    public class BreachedInquiryFun
    {
        [FunctionName("Breaches")]
        public async Task<ActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Queue("azuretestappqueue")] IAsyncCollector<BreachInquiryRequest> verifyBreachRequestQueue,
            CancellationToken cancellationToken,
            ILogger log)
        {
            log.LogInformation("Start processing a verify breach request.");

            var verifyBreachRequest = await req.ReadAsJsonAsync<BreachInquiryRequest>(cancellationToken);

            if (verifyBreachRequest?.Account is null)
                return new BadRequestObjectResult($"Request body is not in a correct format.\nValid format ex:{JsonConvert.SerializeObject(new BreachInquiryRequest { Account = "Your account name" })} ");

            if (verifyBreachRequest?.Account.Trim().Length == 0)
                return new BadRequestObjectResult($"{nameof(verifyBreachRequest.Account)} must have a value!");

            await verifyBreachRequestQueue.AddAsync(verifyBreachRequest);

            log.LogInformation("Verify breach request added to the queue.");

            return new OkObjectResult($"Your request for account: {verifyBreachRequest.Account} registered successfully.");
        }

    }
}
