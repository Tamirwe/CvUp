using CvUpAPI;
using CvUpAPI.Startup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

string CorsPolicy = "_corsPolicy";

builder.Services.AddCors(options => options.AddPolicy(name: CorsPolicy,
                      builder =>  builder.WithOrigins("http://localhost:3030",
                     "http://192.168.1.23:8020",
                     "http://82.166.239.93:8020",
                     "http://10.100.102.23:8020",
                     "http://82.166.239.93:8030",
                     "http://192.168.1.23:8030",
                     "https://localhost:7218").AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        //ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer",
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
});

// Add services to the container.
builder.Services.RegisterServices(builder);

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(CorsPolicy);
//app.UseCors(builder =>
//{
//    builder
//    .AllowAnyOrigin()
//    .AllowAnyMethod()
//    .AllowAnyHeader();
//});

// Configure the HTTP request pipeline.
app.ConfigureSwagger();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

app.UseAuthentication();

app.Use(async (context, next) => {
    if (context.User != null && context.User.Identity != null &&  context.User.Identity != null && context.User.Identity.IsAuthenticated )
    {
        var claimsIdentity = context.User.Identity as ClaimsIdentity;

        if (claimsIdentity != null)
        {
            Globals.CompanyId =  Int32.Parse(claimsIdentity.FindFirst("CompanyId")!.Value);
            Globals.UserId =  Int32.Parse(claimsIdentity.FindFirst("UserId")!.Value);
        }
    }

    await next();
});

app.UseAuthorization();

app.MapControllers();

app.Run();
