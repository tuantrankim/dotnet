https://app.pluralsight.com/library/courses/aspnetcore-mvc-efcore-bootstrap-angular-web/table-of-contents

```
Install Microsoft .Net Core sdk
>dotnet --version
>dotnet new --help

>md helloworld
>cd helloworld
>dotnet new console
>dotnet build
>dotnet run

>dotnet add package NewtonSoft.Json



//Startup.cs
public void Configure(IApplicationBuilder app, IHostingEnvironment evn)
{
  app.Run(async (context) =>{
    await conext.Response.WriteAsync("<html><body>Hello world</body></html>");//write hello world on screen
  });
}

//Startup.cs
public void Configure(IApplicationBuilder app, IHostingEnvironment evn)
{
  // Add middle ware - order is important
  app.UseDefaultFiles();//E.g index.html is a default file
  app.UseStaticFiles();//use static file under folder wwwroot
  app.UseNodeModules();//mapping node module to wwwroot
}


//Startup.cs
public void ConfigureServices(IServiceCollection services)
{
  services.AddControllersWithViews();
}

public void Configure(IApplicationBuilder app, IHostingEnvironment evn)
{
  // Add middle ware - order is important
  /*
#if DEBUG
  app.UseDevelopterExceptionPage();
#endif
  //similar to use env.IsDevelopment()
  //or env.IsEnvironment("Development")
*/
  if(env.IsDevelopment())
  {
    app.UseDeveloperExceptionPage();
  }
  else
  {
    // Add Error Page
    app.UseExceptionHandler("/error");
  }

  app.UseStaticFiles();//use static file under folder wwwroot
  app.UseNodeModules();//mapping node module to wwwroot
  app.UseRouting();
  app.UseEndpoints(cfg=>{
    cfg.MapControllerRoute("Fallback",
      "{controller}/{action}/{id?}",
      new { controller = "App", action = "Index"});
  });
}


//AppController.cs
public IActionResult Index()
{
  //throw new InvalidOperationException();
  return View();
}
```

##Layout page

```
//Views/Shared/_Layout.cshtml
<section>
  <h2>@ViewBag.Title</h2>
  @RenderBody()
</section>

//Views/_ViewStart.cshtml
// execute in every view when it renders
@{
  Layout = "_Layout"; //Setting the layout to be _Layout.cshtml file
}
```

##Controller

```
//exceptional routing
[HttpGet("contact")]
public IActionResult Contact()
{
  ViewBag.Title = "Contact Us";
  return View();
}

public IActionResult About()
{
  ViewBag.Title = "About Us";
  return View();
}
```

##View Import
```
//Views/_ViewImports.cshtml
@Using Hello.Controllers
@addTagHelper "*, Microsoft.AspNetCore.Mvc.TagHelpers"

//Then we can use asp- in cshtml file
<menu>
  <ul>
    <li><a href="/">Home</a></li>
    <li><a href="/app/contact">Contact</a></li>
    <li><a href="/app/about">About</a></li>
   </ul>
</menu>

//using tag helper

<menu>
  <ul>
    <li><a asp-controller="App" asp-action="Index">Home</a></li>
    <li><a asp-controller="App" asp-action="Contact">Contact</a></li>
    <li><a asp-controller="App" asp-action="About">About</a></li>
   </ul>
</menu>

```

##Razor Pages
```
Create pages: Pages\Error.cshtml

@page
<h2>Sorry...</h2>
<p>We've had a problem. Please retry later</p>

```

##MVC http post
//Contact.cshtml
View: Use form method = "post"
Add names to all input taga that need on server side 
Controller: Add [HttpPost("contact"]
```
<h2>@ViewBag.Ttile</h2>
<form method="post">
  <label>Your Name:</label>
  <br/>
  <input name="name"/>
  <br/>
  
  <label>Email:</label>
  <br/>
  <input type="email" name="email"/>
  <br/>
  
  <label>Subject:/label>
  <br/>
  <input type="text" name="subject"/>
  <br/>
  
  <textarea rows="4" name="message"></textarea>
  <br />
  
  <input type="submit" value="Send Message" />
</form>


//AppController.cs
[HttpPost("contact")]
public IActionResult Contact(object model)
{
  return View();
}
```
##Model Binding
//ViewModels/ContactViewModel.cs
with ContactViewModel we can use the class in the post controller action
in _ViewImports.cshtml add @using Hello.ViewModels
in View Contact.cshtml add @model ContactViewModel
similarly, instead of using name, using asp-for to comunicate with action post paramter 

```
//Contact.cshtml
@model ContactViewModel
@{
  ViewBag.Title = "Contact Us";
}

<h2>@ViewBag.Ttile</h2>
<form method="post">
  <label asp-for="Name">Your Name:</label>
  <br/>
  <input asp-for="Name"/>
  <br/>
  
  <label asp-for="Email">Email:</label>
  <br/>
  <input asp-for="Email" type="email"/>
  <br/>
  
  <label asp-for="Subject">Subject:/label>
  <br/>
  <input asp-for="Subject" type="text"/>
  <br/>
  
  <textarea asp-for="Message"" rows="4"></textarea>
  <br />
  
  <input type="submit" value="Send Message" />
</form>

```
```
[HttpPost("contact")]
public IActionResult Contact(ContactViewModel model)
{
  return View();
}
```

```
public class ContactViewModel
{
  public string Name { get; set; }
  public string Email { get; set; }
  public string Subject { get; set; }
  public string Message { get; set; }
}

```

##Validation
//ContactViewModel.cs
```
public class ContactViewModel
{
  [Required]
  [MinLength(5)]
  public string Name { get; set; }
  [Required]
  [EmailAddress]
  public string Email { get; set; }
  [Required]
  public string Subject { get; set; }
  [Required]
  [MaxLength(250), ErrorMessage = "Too Long"]
  public string Message { get; set; }
}
```

##ModelState
using ModelState to check form valid or not
ModelState.ErrorCount shows number of errors
Add to view
```
  <div asp-validation-summary="All"></div>
  OR to show entire view model problems
  <div asp-validation-summary="ModelOnly"></div>
  
  <span asp-validation-for="Name"></span>
```
```
[HttpPost("contact")]
public IActionResult Contact(ContactViewModel model)
{
  if (ModelState.IsValid)
  {
    // Send the email
    _mailService.SendMessage("quoc.nguyen@gmail.com", model.Subject, $"From: {model.Name} - {model.Email}, Message: {model.Message}");
    ViewBag.UserMessage = "Mail Sent";
    ModelState.Clear();//clear the form
    
  }
  else
  {
    // Show the errors
  }
  
  return View();
}
```

##Validation on client side using jquery-validation
```
Similar to adding
  "bootstrap": "^4.1.1",
  "jquery": "^3.1.1",
Add to package.json dependencies
  "jquery-validation": "^1.17.0",
  "jquery-validation-unobtrusive": "^3.2.10"
```

at _Layout.cshtml add @RenderSection("scripts")

```
  <script src="~/node_modules/jquery/dist/jquery.min.js"></script>
  <script srt+"~/js/index.js"></script>
  @RenderSection("scripts", false)
</body>
</html>
```
at Contact.cshtml add @section Scripts

```
@model ContactViewModel
@{
  ViewBag.Title = "Contact Us";
}
@section Scripts {
  <script src="~/node_modules/jquery-validation/dist/jquery.validate.min.js">
  <script src="~/node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.min.js">
</script>
}
```

##Dummy Mail Service - use log
Add file Services/NullMailService.cs
right click NullMailService to extract IMailService interface
```
using Microsoft.Extensions.Logging;
namespace Hello.Services
{
  public class NullMailService: IMailService
  {
    private readonly ILogger<NullMailService> _logger;
    
    public NullMailService(ILogger<NullMailService> logger)
    {
      _logger = logger;
    }
    public void SendMessage(string, to, string subject, string body)
    {
      // Log the message
    
    }
  }
}

//The extract interface
namespace Hello.Services
{
  public interface IMailService
  {
    void SendMessage(string to, string subject, string body);
  }
}

```

Add MailService to ConfigureServices at Startup.cs file

```
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddTransient<IMailService, NullMailService>();
    // Support for real mail service
  }
```

Inject MailService into controller at file AppController.cs
```
  private readonly IMailService _mailService;
  public AppController(IMailService mailService)
  {
    _mailService = mailService; 
  }
```

##Bootstrap
```
Package.json include dependencies
"bootstrap": "^4.1.1"
_Layout.cshtml add Link to css and add JS
<link href="~/node_modules/bootstrap/dist/css/bootstrap.min.css" rel="stylesheet" />
at the end of the file right after jquery script, add bootstrap JS
<script src="~/node_modules/bootstrap/dist/js/bootstrap.bundle.min.js"></script>



```
