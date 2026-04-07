using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    /// <summary>
    /// Сервис передачи уведомлений на сторону клиента о прогрессе выполнения обработки
    /// </summary>
    public interface IProgressLogger
    {
        /// <summary>
        /// Отправить новое значение
        /// </summary>
        /// <param name="progress">Значение прогресса в %</param>
        /// <returns></returns>
        public Task SendProgress(int progress);
    }
}
