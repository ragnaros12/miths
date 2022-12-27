using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace miths.Models
{
	public class Post : ICloneable
	{
		public class CreatePostModel
		{
			[Required]
			[MinLength(5)]
			public string Title { get; set; } = default!;
			
			[Required]
			public string MarkDown { get; set; } = default!;
			
			[Required]
			public IFormFile Image { get; set; } = default!;

			[Required] public string Category { get; set; } = default!;
		}

		public class EditPostModel
		{
			[Required] public Guid Id { get; set; }
			
			[Required] [MinLength(5)] public string Title { get; set; } = default!;

			[Required] public string MarkDown { get; set; } = default!;

			public IFormFile? Image { get; set; } = default!;

			public string? CurrentImage { get; set; } = default!;
			
			[Required] public string Category { get; set; } = default!;
		}

		public virtual User User { get; set; }
		public Guid Id { get; set; }
		public string Title { get; set; } = default!;
		public string MainImage { get; set; } = default!;
		public string MarkDown { get; set; } = default!;
		public DateTime CreationDate { get; set; } = DateTime.Now;
		public string Category { get; set; } = default!;
		public int Views { get; set; }
		
		public virtual List<Comment> Comments { get; set; } = default!;
		
		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
