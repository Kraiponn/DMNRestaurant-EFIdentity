using DMNRestaurant.Areas.Identity.Data;
using DMNRestaurant.Data;
using DMNRestaurant.Helper;
using DMNRestaurant.Services;
using DMNRestaurant.Services.Repository;
using DMNRestaurant.Services.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var securSettings = new SecuritySettings();
var connectionString = builder.Configuration.GetConnectionString("IdentityDbConnection")
    ?? throw new InvalidOperationException("Connection string 'IdentityDbContextConnection' not found.");

builder.Configuration.Bind(nameof(securSettings));
builder.Services.AddSingleton(securSettings);

builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDbConnection")));

builder
    .Services
    .AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<IdentityContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 3;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(option =>
{
    option.TokenLifespan = TimeSpan.FromHours(1);
});

builder
    .Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            //ValidIssuer = securSettings.Issuer,
            ValidateAudience = false,
            //ValidAudience = securSettings.Audience,
            //ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding
                        .UTF8
                        .GetBytes(builder.Configuration.GetSection("SecuritySettings:JwtSecretKey").Value)
            ),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme, e.g. \"bearer {token} \"",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    c.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddScoped<IPhotoRepository, PhotoRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISecurityRepository, SecurityRepository>();

var app = builder.Build();

// Global cors policy
app.UseCors(options => options
    //.WithOrigins("https://example.com", "https://codemakerlab.com") // Allow some domain
    .AllowAnyOrigin() // Allow all domain
    .AllowAnyHeader()
    .AllowAnyMethod()
);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication(); ;
app.UseAuthorization();

app.MapControllers();

app.Run();
