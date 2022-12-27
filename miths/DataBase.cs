using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using miths.Models;

namespace miths
{
	public class DataBase : IdentityDbContext<User, Role, string>
	{

		public DataBase(DbContextOptions<DataBase> options) : base(options)
		{
			Database.EnsureCreated();

			if (!Roles.Any())
			{
				Roles.Add(new Role() { Name = "User", Id = "0", NormalizedName = "USER"});
				Roles.Add(new Role() { Name = "Editor", Id = "1", NormalizedName = "EDITOR"});
				SaveChanges();
			}
		}
		
		public DbSet<Post> Posts { get; set; }
		public DbSet<Comment> Comments { get; set; }
	}
}
