using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using SharepointAgent.Components;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient(); // Basic HttpClient injection
builder.Services.AddHttpClient("AIClient", client =>
{
    client.BaseAddress = new Uri("https://aif-gowtham-dev.openai.azure.com/openai/deployments/text-embedding-3-small/embeddings?api-version=2023-05-15");
    var subscriptionKey = builder.Configuration["AIClient:OcpApimSubscriptionKey"];
    if (!string.IsNullOrEmpty(subscriptionKey))
    {
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
    }
});


builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI(); 
builder.Services.AddRazorPages();

builder.Services.AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map default Razor Pages for Microsoft Identity UI
app.MapControllers();
app.MapRazorPages();

app.Run();
