using ArticleCheck.WebApi.Context;
using ArticleCheck.WebApi.Libraries;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        // React uygulamanýzýn çalýþtýðý adresi belirtin
        policy.WithOrigins("http://localhost:3000")  // React app'inin domain'i
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApiContext>();

builder.Services.AddHttpClient();

builder.Services.AddScoped<ServiceAddReview>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowLocalhost");

//app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, proxy-revalidate";
        ctx.Context.Response.Headers["Pragma"] = "no-cache";
        ctx.Context.Response.Headers["Expires"] = "0";
    }
});
app.UseRouting();

app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
