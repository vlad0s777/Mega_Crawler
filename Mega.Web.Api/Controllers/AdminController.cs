namespace Mega.Web.Api.Controllers
{
    using Mega.Messaging;
    using Mega.Services.UriRequest;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Управляющий контроллер
    /// </summary>
    /// <remarks>
    /// В данном контроллере можно запустить процесс краулинга
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IMessageBroker<UriRequest> broker;

        /// <param name="broker">Брокер сообщений</param>
        public AdminController(IMessageBroker<UriRequest> broker)
        {
            this.broker = broker;
        }

        /// <summary>
        /// Получение списка тегов
        /// </summary>
        /// <remarks>
        /// Брокеру отправляется пустое сообщение, которое инициирует процесс краулинга
        /// </remarks>
        /// <returns>
        /// Результат запуска краулинга
        /// </returns>
        [HttpGet("start")]
        [Authorize(Policy = "RequireAdmin")]
        public string StartCrawler()
        {
            if (this.broker.IsEmpty())
            {
                this.broker.Send(new UriRequest(string.Empty));
                return "Crawler started successfully!";
            }
            else
            {
                return "Crawler is already running";
            }
        }
    }
}