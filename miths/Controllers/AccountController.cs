using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using miths.Models;

namespace miths.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private UserManager<User> _userManager;
    private SignInManager<User> _signInManager;
    private DataBase _dataBase;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, DataBase dataBase)
    {
        _dataBase = dataBase;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<ActionResult> Register(List<string>? errors)
    {
        if (_signInManager.IsSignedIn(User))
            ViewBag.user = await _signInManager.UserManager.GetUserAsync(User);
        ViewBag.IsAdmin = User.IsInRole("Editor");
        ViewBag.errors = errors;
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Register(User.CreateUserModel userModel)
    {
        if (!ModelState.IsValid)
            return await Register(ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage).ToList());
        var user = new User { Email = userModel.Email, UserName = userModel.UserName, Id = Guid.NewGuid().ToString(), IsSend = userModel.IsSend};
        var result = await _userManager.CreateAsync(user,
            userModel.Password);
        if (!result.Succeeded)
            return await Register(result.Errors.Select(u => u.Description).ToList());
        await _userManager.AddToRoleAsync(user, "User");
        return Redirect(nameof(Login));
    }

    public async Task<ActionResult> Login(List<string> errors)
    {
        ViewBag.errors = errors;
        if (_signInManager.IsSignedIn(User))
            ViewBag.user = await _signInManager.UserManager.GetUserAsync(User);
        ViewBag.IsAdmin = User.IsInRole("Editor");
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Login(User.LoginUserModel userModel)
    {
        if (!ModelState.IsValid)
            return await Login(ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage).ToList());
        var result = await _signInManager.PasswordSignInAsync(userModel.Email, userModel.Password, true, false);
        if(!result.Succeeded)
            return await Login( new List<string>{ "User with this password and login does not exists" });
        if (_signInManager.IsSignedIn(User))
            ViewBag.user = await _signInManager.UserManager.GetUserAsync(User);
        ViewBag.IsAdmin = User.IsInRole("Editor");
        return Redirect("~/Home/Index");
    }
    
    public async Task<ActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Redirect(nameof(Login));
    }

    public ActionResult AccessDenied()
    {
        ViewBag.IsAdmin = User.IsInRole("Editor");
        return Ok("У вас нет на это прав");
    }

}