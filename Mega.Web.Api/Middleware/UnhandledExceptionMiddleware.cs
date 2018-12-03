namespace Mega.Web.Api.Middleware
{
    using System;
    using System.Threading.Tasks;

    using Mega.Web.Api.Exceptions;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class UnhandledExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<UnhandledExceptionMiddleware> logger;

        public UnhandledExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.logger = loggerFactory?.CreateLogger<UnhandledExceptionMiddleware>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.next(context);
            }
            catch (HttpResponseException ex)
            {
                if (context.Response.HasStarted)
                {
                    this.logger.LogWarning("The response has already started, the http status code middleware will not be executed.");
                    throw;
                }

                this.logger.LogError($"Message: {ex.Message}. Code: {ex.StatusCode}");
                context.Response.Clear();
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = ex.ContentType;

                await context.Response.WriteAsync(ex.Message);
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message + e.StackTrace);
            }
        }
    }
}
