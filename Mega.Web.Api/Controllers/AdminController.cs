namespace Mega.Web.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Mega.Data.Migrations;
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
    public class AdminController : ControllerBase
    {
        private readonly IMessageBroker<UriRequest> broker;

        private readonly ISomeReportDataProvider someReportDataProvider;

        /// <param name="broker">Брокер сообщений</param>
        /// <param name="someReportDataProvider">Контекст данных</param>
        public AdminController(IMessageBroker<UriRequest> broker, ISomeReportDataProvider someReportDataProvider)
        {
            this.broker = broker;
            this.someReportDataProvider = someReportDataProvider;
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

        /// <summary>
        /// Запуск миграции базы данных
        /// </summary>
        /// <remarks>
        /// В начале он через Dapper запрашивает всё содержимое таблицы миграций; 
        /// Если таблицы нет — создаёт её прямым SQL-запросом через имеющийся DbConnection;
        /// Получает список файлов из каталога миграций, упорядочивает его по алфавиту(и как следствие, по времени);
        /// По порядку пытается выполнять содержимое тех файлов-запросов, которых ещё не было в таблице миграций;
        /// На каждую успешно выполненную миграцию записывает соответствующую строку в таблицу миграций;
        /// </remarks>
        /// <returns>
        /// Результат запуска миграции
        /// </returns>
        [HttpGet("migration")]
        public async Task<string> StartMigration()
        {
            return await this.someReportDataProvider.Migrate();
        }

        /// <summary>
        /// Удаление одного тега
        /// </summary>
        /// <remarks>
        /// Добавляем тег в блеклист (таблица TagsDelete)
        /// </remarks>
        /// <returns>
        /// Результат  удаления
        /// </returns>
        /// <param name="id">Идентификатор тега</param>
        [HttpDelete("deletetag")]
        public async Task<string> DeleteTag(int id)
        {
            object entity;
            try
            {
                entity = new RemovedTag() { DeletionDate = DateTime.Now, Tag = await this.someReportDataProvider.GetTag(id) };
            }
            catch (Exception e)
            {
                return $"Tag {id} no delete. Cause: {e.Message}";
            }

            await this.someReportDataProvider.AddAsync(entity);
            return $"Tag {id} delete.";
        }

        /// <summary>
        /// Удаление списка тегов
        /// </summary>
        /// <remarks>
        /// Добавляем теги в блеклист (таблица TagsDelete)
        /// </remarks>
        /// <returns>
        /// Результат  удаления
        /// </returns>
        /// <param name="ids">Список идентификаторов тегов</param>
        [HttpDelete("deletetags")]
        public async Task<string> DeleteTags(List<int> ids)
        {
            var output = string.Empty;
            foreach (var id in ids)
            {
                output += await DeleteTag(id) + " ";
            }

            return output;
        }
    }
}