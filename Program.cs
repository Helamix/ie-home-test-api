var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
	options.AddPolicy(MyAllowSpecificOrigins, policy =>
	{
		policy.WithOrigins("http://localhost:3000")  // Match exactly
			  .AllowAnyHeader()
			  .AllowAnyMethod()
			  .WithExposedHeaders("Content-Disposition") // Optional: expose custom headers
			  .SetIsOriginAllowed(origin => true) // Optional: Allow dynamic origins (be careful)
			  .AllowCredentials();  // If using cookies/auth headers
	});
});

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
