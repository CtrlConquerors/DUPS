using DUPSS.Web.Components;
using DUPSS.Web.Components.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
// Removed: using Microsoft.AspNetCore.Components.Forms; // This using directive is no longer needed here
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = true;
    });

// Removed: Configure InputFile options here. Max file size will now be handled in the component.
// builder.Services.Configure<InputFileOptions>(options =>
// {
//     options.MaxAllowedSize = 5 * 1024 * 1024; // Allows files up to 5 MB. Adjust this value as needed.
// });


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Auth:Issuer"],
        ValidAudience = builder.Configuration["Auth:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Auth:Token"]!))
    };
});

builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddScoped<JwtAuthenticationStateProvider>(); // For direct injection if needed

builder.Services.AddAuthenticationCore();
builder.Services.AddAuthorizationCore();
builder.Services.AddHttpContextAccessor();

// Configure a single named HttpClient for all API services
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7288/"); // Your API URL
});

// Register API services without AuthService dependency
builder.Services.AddScoped<AuthApiService>(sp => new AuthApiService(
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));
builder.Services.AddScoped<CourseApiService>(sp => new CourseApiService(
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));
builder.Services.AddScoped<CourseTopicApiService>(sp => new CourseTopicApiService(
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));
builder.Services.AddScoped<RoleApiService>(sp => new RoleApiService(
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));
builder.Services.AddScoped<UserApiService>(sp => new UserApiService(
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));
builder.Services.AddScoped<CampaignApiService>(sp => new CampaignApiService(
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));
builder.Services.AddScoped<BlogApiService>(sp => new BlogApiService(
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));
builder.Services.AddScoped<BlogTopicApiService>(sp => new BlogTopicApiService(
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));
builder.Services.AddScoped<AppointmentApiService>(sp => new AppointmentApiService(
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));
builder.Services.AddScoped<CourseEnrollApiService>(sp => new CourseEnrollApiService(
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));
builder.Services.AddScoped<CampaignRegistrationApiService>(sp => new CampaignRegistrationApiService(
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));


builder.Services.AddScoped<ProtectedLocalStorage>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowApi", builder =>
        builder.WithOrigins("https://localhost:7288")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// REMOVED: This generic HttpClient registration is likely redundant and could cause issues
// if the named "ApiClient" is intended for all API calls.
// builder.Services.AddScoped(sp => new HttpClient
// {
//     BaseAddress = new Uri("https://localhost:7084/")
// });

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    app.UseHsts();
}

app.UseCors("AllowApi");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
