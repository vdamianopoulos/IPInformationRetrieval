using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace IPInformationRetrieval.Validators
{
    public class ValidateIpAddress : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var ipAddress = context.RouteData.Values["ip"]?.ToString();

            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                context.Result = new BadRequestObjectResult("Empty IP address provided");
                return;
            }
            if (!IsValidIpAddress(ipAddress))
            {
                context.Result = new BadRequestObjectResult("Invalid IP address provided");
                return;
            }

            base.OnActionExecuting(context);
        }

        private static bool IsValidIpAddress(string ip) => IPAddress.TryParse(ip, out _);
    }
}
