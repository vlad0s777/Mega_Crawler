namespace Mega.Web.Api.Controllers
{
    using Mega.Domain;
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
    [Authorize(Policy = "RequireAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageBroker<UriRequest> broker;

        private readonly IDataContext context;

        /// <param name="broker">Брокер сообщений</param>
        /// <param name="context">Контекст данных</param>
        public MessagesController(IMessageBroker<UriRequest> broker, IDataContext context)
        {
            this.broker = broker;
            this.context = context;
        }

        /// <summary>
        /// Запуск краулера
        /// </summary>
        /// <remarks>
        /// Брокеру отправляется пустое сообщение, если таблица тегов не пуста,
        /// или сообщение tags, если пуста
        /// </remarks>
        /// <returns>
        /// Результат запуска краулинга
        /// </returns>
        [HttpPost("start")]
        public string StartCrawler()
        {
            if (this.broker.IsEmpty())
            {
                this.broker.Send(this.context.CountTags() != 0 ? new UriRequest(string.Empty) : new UriRequest("tags"));

                return "Crawler started successfully!";
            }
            else
            {
                return "Crawler is already running";
            }
        }
    }
}