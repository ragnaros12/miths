using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace miths.Models;

public class Comment
{
    public Guid Id { get; set; }
    public virtual User Author { get; set; } = default!;
    public string Message { get; set; } = default!;
    public virtual DateTime CreationDate { get; set; }
}