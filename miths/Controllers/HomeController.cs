using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using miths.Models;

namespace miths.Controllers;

public class HomeController : Controller
{
    private DataBase _dataBase;
    private SignInManager<User> _signInManager;
    private readonly int CountPages = 6;
    private readonly int PostPerPage = 12;

    public HomeController(DataBase dataBase, SignInManager<User> signInManager)
    {
        _dataBase = dataBase;
        _signInManager = signInManager;
    }

    
    public async Task<IActionResult> Index(string? category, int page = 1)
    {
        if (page < 1)
            return Redirect(nameof(Index));
        if (_signInManager.IsSignedIn(User))
            ViewBag.user = await _signInManager.UserManager.GetUserAsync(User);
        ViewBag.IsAdmin = User.IsInRole("Editor");

        var posts = _dataBase.Posts.ToList();
        var categories = posts.Select(u => u.Category).Distinct().ToList();
        if (category != null)
            posts = posts.Where(u => u.Category == category).ToList();


        return View(new ShowPosts
        {
            Categories = categories, Posts = posts.Skip((page - 1) * PostPerPage).Take(PostPerPage).ToList(), page = page, IsNext = posts.Count > page * PostPerPage, IsPrev = page != 1,
            Category = category == null ? null : "&category=" + category
        });
    }
    

}