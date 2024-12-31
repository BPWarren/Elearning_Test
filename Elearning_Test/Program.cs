
using Elearning_Test.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Ajouter les services Identity et la base de donn�es
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Ajouter la gestion des r�les
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Cr�er les r�les au d�marrage
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roles = { "Admin", "Professeur", "Etudiant" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var prof = new IdentityUser { UserName = "professeur@example.com", Email = "professeur@example.com", EmailConfirmed = true };
    var student = new IdentityUser { UserName = "etudiant@example.com", Email = "etudiant@example.com", EmailConfirmed = true };
    var admin = new IdentityUser { UserName = "admin@example.com", Email = "admin@example.com", EmailConfirmed = true };

    if (await userManager.FindByEmailAsync(prof.Email) == null)
    {
        await userManager.CreateAsync(prof, "Password123!");
        await userManager.AddToRoleAsync(prof, "Professeur");
    }

    if (await userManager.FindByEmailAsync(student.Email) == null)
    {
        await userManager.CreateAsync(student, "Password123!");
        await userManager.AddToRoleAsync(student, "Etudiant");
    }

    if (await userManager.FindByEmailAsync(admin.Email) == null)
    {
        await userManager.CreateAsync(admin, "Password123!");
        await userManager.AddToRoleAsync(admin, "Admin");
    }
}


app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run();