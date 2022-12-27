namespace miths.Models;

public class ShowPosts
{
    public List<string> Categories { get; set; }
    public List<Post> Posts { get; set; }

    public int page { get; set; } = 1;
    public string? Category { get; set; }

    public bool IsNext { get; set; }
    public bool IsPrev { get; set; }
}