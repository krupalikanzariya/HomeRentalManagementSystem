using HomeRentalAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<UsersRepository>();
builder.Services.AddScoped<PropertiesRepository>();
builder.Services.AddScoped<BookingsRepository>();
builder.Services.AddScoped<ReviewsRepository>();
builder.Services.AddScoped<AmenitiesRepository>();
builder.Services.AddScoped<PropertyAmenitiesRepository>();
builder.Services.AddScoped<ImagesRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
