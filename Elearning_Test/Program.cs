

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Elearning_Test.Data;
using Elearning_Test.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    context.Database.EnsureCreated();

    // Seed roles
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }
    if (!await roleManager.RoleExistsAsync("Professeur"))
    {
        await roleManager.CreateAsync(new IdentityRole("Professeur"));
    }
    if (!await roleManager.RoleExistsAsync("Etudiant"))
    {
        await roleManager.CreateAsync(new IdentityRole("Etudiant"));
    }

    if (!context.Admins.Any())
    {
        // Seed an admin user
        var adminUser = new Admin
        {
            UserName = "admin@example.com",
            Email = "admin@example.com",
            Nom = "Admin",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var result = await userManager.CreateAsync(adminUser, "Admin@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // Seed a professeur
        var professeur = new Professeur
        {
            UserName = "professeur@example.com",
            Email = "professeur@example.com",
            Nom = "Professeur",
            Prenom = "Test",
            Specialite = "Informatique",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        result = await userManager.CreateAsync(professeur, "Professeur@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(professeur, "Professeur");
        }

        // Seed an etudiant
        var etudiant = new Etudiant
        {
            UserName = "etudiant@example.com",
            Email = "etudiant@example.com",
            Cne = "E123456789",
            Nom = "Etudiant",
            Prenom = "Test",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        result = await userManager.CreateAsync(etudiant, "Etudiant@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(etudiant, "Etudiant");
        }

        // Seed a categorie
        var categorie = new Categorie
        {
            Intitule = "Informatique",
            Description = "Cours d'informatique",
            ImageFile = "informatique.jpg",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Categories.Add(categorie);
        await context.SaveChangesAsync();

        // Seed a cours
        var cours = new Cours
        {
            Titre = "Introduction à la programmation",
            Description = "Cours d'introduction à la programmation",
            ProfesseurId = professeur.Id,
            Professeur = professeur, // Initialisation obligatoire
            Price = 100,
            CategorieId = categorie.Id,
            Categorie = categorie, // Initialisation obligatoire
            ImageFile = "programmation.jpg",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Cours.Add(cours);
        await context.SaveChangesAsync();

        // Seed a lecon
        var lecon = new Lecon
        {
            Titre = "Variables et types de données",
            Contenu = "Contenu de la leçon sur les variables et types de données",
            NumeroPage = 1,
            CoursId = cours.Id,
            Cours = cours, // Initialisation obligatoire
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Lecons.Add(lecon);
        await context.SaveChangesAsync();

        // Seed an enrollment
        var enrollment = new Enrollment
        {
            EtudiantId = etudiant.Id,
            Etudiant = etudiant, // Initialisation obligatoire
            CoursId = cours.Id,
            Cours = cours, // Initialisation obligatoire
            Progression = 0,
            IsConnected = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Enrollments.Add(enrollment);
        await context.SaveChangesAsync();

        // Seed a payment
        var payment = new Payment
        {
            OwnerName = "Etudiant Test",
            EtudiantId = etudiant.Id,
            Etudiant = etudiant, // Initialisation obligatoire
            CoursId = cours.Id,
            Cours = cours, // Initialisation obligatoire
            Amount = 100,
            PaymentDate = DateTime.UtcNow,
            CVC = 123,
            NumeroCarte = "1234567890123456",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Payments.Add(payment);
        await context.SaveChangesAsync();

        // Seed a certification
        var certification = new Certification
        {
            EtudiantId = etudiant.Id,
            Etudiant = etudiant, // Initialisation obligatoire
            CoursId = cours.Id,
            Cours = cours, // Initialisation obligatoire
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Certifications.Add(certification);
        await context.SaveChangesAsync();

        // Seed an evaluation
        var evaluation = new Evaluation
        {
            Content = "Très bon cours",
            CoursId = cours.Id,
            Cours = cours, // Initialisation obligatoire
            EtudiantId = etudiant.Id,
            Etudiant = etudiant, // Initialisation obligatoire
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Evaluations.Add(evaluation);
        await context.SaveChangesAsync();
    }
}

app.Run();