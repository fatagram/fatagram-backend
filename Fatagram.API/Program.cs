using Fatagram.Application.Common;
using Fatagram.Application.Services.ImageService;
using Fatagram.Application.Services.ImageService.Interface;

namespace Fatagram.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            

            // Dependency injection
            builder.Services.AddHttpContextAccessor();


            // Scoped for services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserPrivacyService, UserPrivacyService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IJwtService, JwtHmacSha256Service>();
            builder.Services.AddScoped<IImageService, WwwrootImageService>();


            builder.Services.AddScoped<IUserPrivacyRepository, UserPrivacyRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            builder.Services.AddAutoMapper(typeof(Mapping));


            // Singleton
            builder.Services.AddSingleton<JwtHmacSha256Service>();
            builder.Services.AddSingleton(TimeProvider.System);


            // Turn off ModelStateInvalidFilter
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var _myAllowSpecificOrigins = "_myAllowSpecificOrigins";


            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: _myAllowSpecificOrigins,
                        builder =>
                        {
                            builder.SetIsOriginAllowed(origin => true)
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                        });
            });

            // Add authentication
            builder.Services.AddAuthentication("JwtAuthenticationScheme")
                .AddScheme<AuthenticationSchemeOptions, JwtAuthenticationHandler>("JwtAuthenticationScheme", null);

            var certPath = builder.Configuration["PfxPath"] ?? "";
            var certPassword = "chaungocphat123";

            // Add authorization
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5000);
                options.ListenAnyIP(5001, listenOptions =>
                {
                    listenOptions.UseHttps(certPath, certPassword);
                });
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseCors(_myAllowSpecificOrigins);
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            // Check access token
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
