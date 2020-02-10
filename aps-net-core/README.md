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


useful boostrap class
<body>
<header class="container">
  <nav class="navbar navbar-dark bg-dark navbar-expand-md">
    <h1 class="navbar-brand">Welcome to DEMO</h1>
    <button class="navbar-toggler" data-toggle="collapse" data-target="#theMenu">
      <span class="navbar-toggler-icon"></span>
    </button>
    <div id="theMenu" class="navbar-collapse collapse">
      <ul class="navbar-nav">
        <li class="nav-item">
          <a class="nav-link" asp-controller="App" asp-action="Index">Home</a>
          <a class="nav-link" asp-controller="Contact" asp-action="Contact">Contact</a>
          <a class="nav-link" asp-controller="About" asp-action="About">About</a>
        </li>
      </ul>
    </div>
  </nav>
 </header>
</body>
</header>
  
```
##Bootstrap Grid
```
12 cols example
.col-9  .col-3
.col-8.offset-2

.col-xl-xx  >=1200px  desktop
.col-lg-xx  >=992px laptop
.col-md-xx  >=768px ipad
.col-sm-xx  <768px  phone
.col-xx     <576px  very small phone

Example:
In _Layout.cshtlm

<h2 class="text-center">@ViewBag.Title</h2>

In Contact.cshtml

<div class="row">
  <div class="col-md-6 offset-md-3">
    <div class="card card-body bg-light">
      <form method="post">
      </form>
    </div>
  </div>
  <div class="col-md-3">
  </div>
</div>
```

##Bootstrap Forms
```
use class: form-group, form-control

<div class="form-group">
  <label asp-for="Email">Email:</label>
  <input type="email" asp-for="Email" class="form-control" />
  <span asp-validation-for="Email" class="text-danger"></span>
</div>


<div class="form-group">
  <label asp-for="Subject">Subject:</label>
  <input type="text" asp-for="Subject" class="form-control" />
  <span asp-validation-for="Subject"></span>
</div>

<div class="form-group">
  <input type="submit" value="Send Message" class="btn btn-primary" />
  <div class="text-success">@ViewBag.UserMessage</div>
</div>

```
##Font awesome
```
Add a dependency to package.json
"font-awesome": "^4.7.0"
Add link to font-awesome.min.css
<link href="~/node_modules/font-awesome/css/font-awesome.min.css" rel="stylesheet"/>
example using
<i class="fa fa-envelop"></i>
```

#Entity framework core
```
Add new file
Data/Entities/
Clone or download from github
https://github.com/psauthor/BuildingASPNETCore2
Add some resources: Product.cs, Order.cs, OrderItem.cs
Add Nuget package:
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Design
Create context object
```

```
//Data/Entities/Product.cs
namespace Hello.Data.Entities
{
  public class Product
  {
    public int Id {get; set;}
    public string Category {get; set;}
    public string Size {get; set;}
    public decimal Price {get;set;}
    public string Title {get; set;}
    public string ArtDescription {get; set;}
    public string ArtDating {get; set;}
    public string ArtId {get; set;}
    public string Artist {get; set;}
    public DateTime ArtistBirthDate {get; set;}
    public DateTime ArtistDeathDate {get; set;}
    public string ArtistNationality {get; set;}
  }
}
```

```
//Data/Entities/Order.cs
namespace Hello.Data.Entities
{
  public class Order
  {
    public int Id {get; set;}
    public DateTime OrderDate {get; set;}
    public string OrderNumber {get; set;}
    public ICollection<OrderItem> Items {get;set;}
  }
}
```

```
//Data/Entities/OrderItem.cs
namespace Hello.Data.Entities
{
  public class Order
  {
    public int Id {get; set;}
    public Product Product {get; set;}
    public int Quantity {get; set;}
    public Order Order {get;set;}
  }
}
```
```
//Data/HelloContext.cs
namespace Hello.Data
{
  public class HelloContext : DbContext
  {
    public HelloContext(DbContextOptions<HelloContext> options): base(options)
    {
    }
    public DbSet<Product> Products {get; set;}
    public DbSet<Order> Orders {get; set;}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
     // modelBuilder.Entity<Product>()
     //   .Property(p=>p.Title)
     //   .HasMaxLength(50);
     
     // Simple way to seed data. To seed more complex data using a different way after
     // we don't use this way bc this will run everytime we new HelloContext.
     // We want to run this when the application start (only 1 time) to get better performance
     modelBuilder.Entity<Order>()
      .HasData(new Order()
      {
        Id = 1,
        OrderDate = DateTime.UtcNow,
        OrderNumber = "12345"
      });
    }
  }
}
// to generate seed data, run the command
cmd> dotnet ef migrations add SeedDate
```
##Add DbContext to services and configuration
```
//Startup.cs
public class Startup
{
  private readonly IConfiguration _config;
  
  public Startup(IConfiguration config)
  {
    _config = config;
  }
  public void ConfigureServices(IserviceCollection services)
  {
    services.AddDbContext<HelloContext>(cfg => 
    {
      cfg.Use.UseSqlServer(_config.GetConnectionString("HelloConnectionString"));
    });
  }
}

//Program.cs
public static IWebHost BuildWebHost(string[] args)=>
  WebHost.CreateDefaultBuilders(args)
    .ConfigureAppConfiguration(SetupConfiguration)
    .UseStartup<Startup>()
    .Build();
    
private static void SetupConfiguration(WebHostBuilderContext ctx, IConfigurationBuilder builder)
{
  // Removing the default configuration options
  builder.Sources.Clear();
  
  // Structure in hireachy so the overriding will occurr
  builder.AddJsonFile("config.json", false, true)
        //.AddXmlFile("config.xml", true)
        .AddEnvironmentVariables();
}

//add new config.json
{
  "Colors": {
    "Favorite": "blue"
  },
  "ConnectionStrings": {
    "HelloConnectionString": "server=(localdb)\\ProjectsV13;Database=HelloDb;Integrated Security=true;MultipleActiveResultSets=true;"
   }
}
```
##Install dotnet-ef tool in global
cmd> dotnet tool install dotnet-ef -g  
cmd> dotnet ef database update
//to generate seed file
cmd> dotnet ef migrations add SeedDate

##Seeding the Databalse using entity framework
```
//HelloContext.cs
protected
{
}
//Program.cs
public class Program
{
  public static void Main(string[] args)
  {
    var host = BuildWebHost(args);
    //Instantiate and build up the seeder
    RunSeeding(host);
    host.Run();
  }
}

private static void RunSeeding(IWebHost host)
{
  var scopeFactory = host.Services.GetService<IServiceScopeFactory>();
  using (var scope = scopeFactory.CreateScope())
  {
    var seeder = scope.ServiceProvider.GetService<HelloSeeder>();
    seeder.Seed();
  }
}

//Register at Startup.cs
public void ConfigureServices(IServiceCollection services)
{
  services.AddDbContext<HelloContext>(cfg =>
  {
    cfg.UseSqlServer(_config.GetConnectionString("HelloConnectionString"));
  });
  
  services.AddTransient<HelloSeeder>();
}
```
