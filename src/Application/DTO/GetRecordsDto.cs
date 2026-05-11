namespace Application.DTO;

public record GetRecordsDto(
    int Id,
    string Name,
    string OriginalFilePath,
    string ResultImagePath,
    string ResultRoutePath,
    DateTime CreatedAt
);
