namespace Mega.Web.Api.Controllers
{
    using System.Threading.Tasks;

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

        private readonly ISomeReportDataProvider someReportDataProvider;

        /// <param name="broker">Брокер сообщений</param>
        /// <param name="someReportDataProvider">Контекст данных</param>
        public MessagesController(IMessageBroker<UriRequest> broker, ISomeReportDataProvider someReportDataProvider)
        {
            this.broker = broker;
            this.someReportDataProvider = someReportDataProvider;
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
        public async Task<string> StartCrawler()
        {
            if (this.broker.IsEmpty())
            {
                this.broker.Send(await this.someReportDataProvider.CountTags() != 0 ? new UriRequest(string.Empty) : new UriRequest("tags"));

                return "Crawler started successfully!";
            }
            else
            {
                return "Crawler is already running";
            }
        }
    }
}