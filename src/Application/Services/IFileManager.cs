using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    /// <summary>
    /// Сервис чтения/записи изображений
    /// </summary>
    public interface IFileManager
    {
        public Task<bool> CreateFile(string path, IFormFile content);

        public Task<IFormFile> OpenFile(string path);
    }
}
