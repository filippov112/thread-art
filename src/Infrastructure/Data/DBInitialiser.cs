using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Data;

public class DBInitialiser(ApplicationDbContext context)
{
    public async Task InitialiseDatabaseAsync()
    {
        try
        {
            context.Database.Migrate();
            Console.WriteLine("Миграции успешно применены.");
        }
        catch (Exception ex)
        {
            throw new IOException($"Ошибка при применении миграций: {ex}");
        }
    }
}

