using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudyPlanner.Data;
using StudyPlanner.Models;
using StudyPlanner.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Контекст БД
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    options.User.RequireUniqueEmail = true;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Cookie config (как в методичке)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

// Сервисы
builder.Services.AddScoped<IAssignmentService, AssignmentService>();
var app = builder.Build();



if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// default route как в методичке
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Assignment}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    context.Database.Migrate();

    if (!context.Users.Any())
    {
        var user = new User
        {
            UserName = "testuser",
            Email = "test@example.com",
            CreatedAt = DateTime.UtcNow
        };
        await userManager.CreateAsync(user, "Test123!");

        var subjects = new[]
        {
            new Subject { Name = "Математика", Color = "FF5733", OwnerId = user.Id },
            new Subject { Name = "Физика", Color = "33FF57", OwnerId = user.Id },
            new Subject { Name = "Программирование", Color = "3357FF", OwnerId = user.Id }
        };
        context.Subjects.AddRange(subjects);
        await context.SaveChangesAsync();

        var tags = new[]
        {
            new Tag { Name = "Важное", OwnerId = user.Id },
            new Tag { Name = "Срочное", OwnerId = user.Id },
            new Tag { Name = "Идея", OwnerId = user.Id }
        };
        context.Tags.AddRange(tags);
        await context.SaveChangesAsync();
    }
}
app.Run();