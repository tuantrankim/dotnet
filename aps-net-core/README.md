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
  <input/>
  <br/>
  
  <label>Email:</label>
  <br/>
  <input type="email"/>
  <br/>
  
  <label>Subject:/label>
  <br/>
  <input type="text"/>
  <br/>
  
  <textarea rows="4"></textarea>
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
