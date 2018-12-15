namespace Mega.Web.Api.Controllers
{
    using System.Threading.Tasks;

    using Mega.Domain.Repositories;
    using Mega.Messaging;
    using Mega.Messaging.MessageTypes;

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

        private readonly ITagRepository tagRepository;

        /// <param name="broker">Брокер сообщений</param>
        /// <param name="tagRepository">Репозиторий тегов</param>
        public MessagesController(IMessageBroker<UriRequest> broker, ITagRepository tagRepository)
        {
            this.broker = broker;
            this.tagRepository = tagRepository;
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
                this.broker.Send(await this.tagRepository.CountTags() != 0 ? new UriRequest(string.Empty) : new UriRequest("tags"));

                return "Crawler started successfully!";
            }
            else
            {
                return "Crawler is already running";
            }
        }
    }
}