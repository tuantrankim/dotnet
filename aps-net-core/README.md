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
public IActionResult Contact()
{
  ViewBag.Title = "Contact Us";
  return View();
}
```
