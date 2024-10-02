using JWT_TOKEN_REFRESH_NET_CORE_8;
using JWT_TOKEN_REFRESH_NET_CORE_8.Contextes;
using JWT_TOKEN_REFRESH_NET_CORE_8.Models;
using JWT_TOKEN_REFRESH_NET_CORE_8.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddDbContext<DBEngine>(options =>
//                 options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionStrings:DefaultConnection"))
//            );

// ... other service configurations

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddDbContext<DBEngine>(options =>
{
    options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings:DefaultConnection").Value);
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "yourIssuer",
        ValidAudience = "yourAudience",
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("082F41538C1178DE768A9AC86291678D"))
    };
});

builder.Services.AddScoped<IAuthService,AuthServices>();



// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Your Angular app origin
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // This is required for credentials like cookies or headers
    });
});

//builder.Services.AddAuthorization(); //Here Added by Rohit Joijode 

//builder.Services.AddControllers();

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

//app.UseCors(builder => builder
//       .AllowAnyHeader()
//       .AllowAnyMethod()
//       .AllowAnyOrigin()
//    );

// Use the CORS policy
app.UseCors("AllowSpecificOrigin");



app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

