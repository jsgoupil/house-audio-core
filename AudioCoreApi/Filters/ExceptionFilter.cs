using AudioCoreApi.Exceptions;
using AudioCoreSerial;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCoreApi.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public ExceptionFilter()
        {
        }

        public override Task OnExceptionAsync(ExceptionContext context)
        {
            ValidationProblemDetails validationProblemDetails = null;
            if (context.Exception is InexistentResourceException)
            {
                validationProblemDetails = new ValidationProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Instance = context.HttpContext.Request.Path,
                    Type = "https://audiocore/errors/inexistent_resource",
                    Title = "Inexistent Resource",
                    Detail = "The request you sent tries to access a resource that does not exist."
                };
            }

            if (context.Exception is SerialPortException)
            {
                validationProblemDetails = new ValidationProblemDetails
                {
                    Status = StatusCodes.Status503ServiceUnavailable,
                    Instance = context.HttpContext.Request.Path,
                    Type = "https://audiocore/errors/serial_port",
                    Title = "Serial Port Exception",
                    Detail = context.Exception.Message
                };
            }

            if (validationProblemDetails != null && validationProblemDetails.Status.HasValue)
            {
                context.Result = new ObjectResult(validationProblemDetails)
                {
                    StatusCode = validationProblemDetails.Status.Value,
                    ContentTypes = { "application/problem+json", "application/problem+xml" }
                };
            }

            return base.OnExceptionAsync(context);
        }
    }
}
