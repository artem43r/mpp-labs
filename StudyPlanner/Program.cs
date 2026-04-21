using Microsoft.EntityFrameworkCore;
using StudyPlanner.Data;
using StudyPlanner.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

SeedData(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static void SeedData(IServiceProvider serviceProvider)
{
    using (var scope = serviceProvider.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Database.Migrate();

        if (context.Users.Any())
        {
            return;
        }

        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "123456"
        };

        context.Users.Add(user);
        context.SaveChanges();

        var subjects = new[]
        {
            new Subject { Name = "Личные дела", Description = "Личные задачи", Color = "FF5733", OwnerId = user.Id },
            new Subject { Name = "Работа", Description = "Рабочие задачи", Color = "33FF57", OwnerId = user.Id },
            new Subject { Name = "Учеба", Description = "Учебные задачи", Color = "3357FF", OwnerId = user.Id }
        };

        context.Subjects.AddRange(subjects);
        context.SaveChanges();

        var tags = new[]
        {
            new Tag { Name = "Важное", OwnerId = user.Id },
            new Tag { Name = "Срочное", OwnerId = user.Id },
            new Tag { Name = "Идея", OwnerId = user.Id }
        };

        context.Tags.AddRange(tags);
        context.SaveChanges();

        var assignments = new[]
        {
            new Assignment
            {
                Title = "Купить продукты",
                Description = "Молоко, хлеб, яйца",
                Status = "New",
                Priority = "Medium",
                UserId = user.Id,
                SubjectId = subjects[0].Id,
                Deadline = DateTime.UtcNow.AddDays(1),
                Recurrence = "None"
            },
            new Assignment
            {
                Title = "Сдать отчет",
                Description = "Подготовить квартальный отчет",
                Status = "InProgress",
                Priority = "High",
                UserId = user.Id,
                SubjectId = subjects[1].Id,
                Deadline = DateTime.UtcNow.AddHours(5),
                Recurrence = "None"
            },
            new Assignment
            {
                Title = "Прочитать книгу",
                Description = "Глава 3",
                Status = "New",
                Priority = "Low",
                UserId = user.Id,
                SubjectId = subjects[2].Id,
                Deadline = null,
                Recurrence = "None"
            }
        };

        context.Assignments.AddRange(assignments);
        context.SaveChanges();

        var assignmentTags = new[]
        {
            new AssignmentTag { AssignmentId = assignments[0].Id, TagId = tags[1].Id },
            new AssignmentTag { AssignmentId = assignments[1].Id, TagId = tags[0].Id },
            new AssignmentTag { AssignmentId = assignments[1].Id, TagId = tags[1].Id },
            new AssignmentTag { AssignmentId = assignments[2].Id, TagId = tags[2].Id }
        };

        context.AssignmentTags.AddRange(assignmentTags);
        context.SaveChanges();

        var comments = new[]
        {
            new Comment { Text = "Не забыть купить всё", AssignmentId = assignments[0].Id, UserId = user.Id },
            new Comment { Text = "Срочно!", AssignmentId = assignments[1].Id, UserId = user.Id }
        };

        context.Comments.AddRange(comments);
        context.SaveChanges();
    }
}