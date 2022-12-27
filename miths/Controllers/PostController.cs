using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using miths.Models;

namespace miths.Controllers;

public class PostController : Controller
{
    private string UploadedFile(IFormFile model)  
    {
        if (!Directory.Exists("wwwroot/ims"))
            Directory.CreateDirectory("wwwroot/ims");
        string filePath = "ims/" +  Guid.NewGuid() + "_" + model.FileName;  
        using (var fileStream = new FileStream("wwwroot/" + filePath, FileMode.Create))  
        {  
            model.CopyTo(fileStream);  
        }  
         
        return filePath;  
    } 
    private string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private DataBase _dataBase;
    private SignInManager<User> _signInManager;
    private MailAddress _from = new("khlopovartem43@gmail.com", "Artem");
    private SmtpClient _smtp = new("smtp.gmail.com", 587);


    public PostController(DataBase dataBase, SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
        _dataBase = dataBase;
        
        
        _smtp.Credentials = new NetworkCredential("khlopovartem43@gmail.com", "klttqksrovbbjgkp");
        _smtp.EnableSsl = true;

    }
    
    [Authorize(Roles = "Editor")]
    public async Task<ActionResult> Add(List<string>? errors)
    {
        if (_signInManager.IsSignedIn(User))
            ViewBag.user = await _signInManager.UserManager.GetUserAsync(User);
        ViewBag.IsAdmin = User.IsInRole("Editor");
        ViewBag.errors = errors;
        return View();
    }
    
    [Authorize(Roles = "Editor")]
    [HttpPost]
    public async Task<ActionResult> Add(Post.CreatePostModel postModel)
    {
        if (!ModelState.IsValid)
            return await Add(ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage).ToList());
        var post = new Post {
            Title = postModel.Title,
            MarkDown = postModel.MarkDown,
            MainImage = UploadedFile(postModel.Image),
            Category = postModel.Category,
            User = await _signInManager.UserManager.GetUserAsync(User),
            Comments = new List<Comment>()
        };
        _dataBase.Posts.Add(post);
        _dataBase.SaveChanges();


        foreach (var user in _dataBase.Users.ToList())
        {
            if (user.IsSend)
            {
                MailAddress to = new MailAddress(user.Email);
                MailMessage m = new MailMessage(_from, to);
                m.Subject = "New post: " + post.Title;
                m.Body = $"Author {post.User.UserName} publish new post: {post.Title}. Check this post by link - https://localhost:7247/Post/Post?id={post.Id}";
                m.IsBodyHtml = true;
                _smtp.Send(m);
            }
        }
        
        return RedirectToAction(nameof(Post), new {id = post.Id});
    }
    
    
    [Authorize(Roles = "Editor")] 
    public async Task<ActionResult> Edit(Guid id, List<string>? errors)
    {
        ViewBag.errors = errors;
        var result = _dataBase.Posts.Where(u => u.Id == id);
        if (!result.Any())
            return NotFound();
        var post = result.First();
        if (_signInManager.IsSignedIn(User))
            ViewBag.user = await _signInManager.UserManager.GetUserAsync(User);
        ViewBag.IsAdmin = User.IsInRole("Editor");
        return View(new Post.EditPostModel() { Title = post.Title, CurrentImage = post.MainImage, Id = post.Id, MarkDown = post.MarkDown});
    }

    [Authorize(Roles = "Editor")]
    [HttpPost]
    public async Task<ActionResult> Edit(Post.EditPostModel postModel)
    {
        if (!ModelState.IsValid)
            return await Edit(postModel.Id, ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage).ToList());

        var result = _dataBase.Posts.Where(u => u.Id == postModel.Id);
        if (!result.Any())
            return NoContent();
        
        

        var post = result.First();
        post.Title = postModel.Title;
        post.MarkDown = postModel.MarkDown;
        post.MainImage = postModel.Image == null ? post.MainImage : UploadedFile(postModel.Image);
        post.Category = postModel.Category;

        _dataBase.Posts.Update(post);
        
        _dataBase.SaveChanges();
        
        if (_signInManager.IsSignedIn(User))
            ViewBag.user = await _signInManager.UserManager.GetUserAsync(User);
        ViewBag.IsAdmin = User.IsInRole("Editor");

        return RedirectToAction(nameof(Post), new { id = postModel.Id });
    }
    
    public async Task<ActionResult> Post(Guid id, List<string>? errors)
    {
        var result = _dataBase.Posts.Where(u => u.Id == id);
        if (!result.Any())
            return NoContent();
        
        if (_signInManager.IsSignedIn(User))
            ViewBag.user = await _signInManager.UserManager.GetUserAsync(User);
        ViewBag.IsAdmin = User.IsInRole("Editor");


        var post = result.First();
        post.Views++;
        _dataBase.Posts.Update(post);
        await _dataBase.SaveChangesAsync();

        ViewBag.errors = errors;
        
        return View(post);
    }

    public class CreatePost
    {
        public string? Message { get; set; }
        
        [Required]
        public Guid IdPost { get; set; }
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> Comment(CreatePost createPost)
    {
        if (!ModelState.IsValid)
            return await Post(createPost.IdPost, ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage).ToList());

        var result = _dataBase.Posts.Where(u => u.Id == createPost.IdPost);
        if (!result.Any())
            return NoContent();
        
        if (_signInManager.IsSignedIn(User))
            ViewBag.user = await _signInManager.UserManager.GetUserAsync(User);
        ViewBag.IsAdmin = User.IsInRole("Editor");   
        
        var post = result.First();
        post.Comments.Insert(0, new Comment() {Message = createPost.Message, Author = ViewBag.user, CreationDate = DateTime.Now});
        _dataBase.Posts.Update(post);
        await _dataBase.SaveChangesAsync();


        return RedirectToAction(nameof(Post), new {id = createPost.IdPost});

    }
}