namespace Application.DTO;

public record ProcessedResultDto(
    int Id,
    string Name,
    string OriginalFilePath,
    string ResultImagePath,
    string ResultRoutePath,
    DateTime CreatedAt
);
