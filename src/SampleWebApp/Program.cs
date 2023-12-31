using AspnetJavascriptIsolation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(); 

builder.AddJavascriptIsolation(options =>
{
	options.CacheDurationInHours = 0;
	var rootPath = System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location);
	options.RootPath = builder.Environment.ContentRootPath;
}); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.UseJavascriptIsolation();

app.Run();
