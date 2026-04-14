using Domain.Enums;

namespace Application.Interfaces
{
    /// <summary>
    /// Сервис передачи уведомлений на сторону клиента о прогрессе выполнения обработки
    /// </summary>
    public interface IProgressLogger
    {
        /// <summary>
        /// Отправить новое значение
        /// </summary>
        public Task SendProgressAsync(ProgressStage stage);
    }
}
