
namespace RiverLine.Api;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddControllers(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true;
                options.Filters.Add<FluentValidationFilter>();
            })
            .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); })
            .AddXmlSerializerFormatters();
        builder.Services.AddProblemDetails();
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        builder.Services.AddSwaggerGen(options =>
        {
            options.AddServer(new OpenApiServer
            {
                Url = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5132"
            });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token."
            });

            options.AddSecurityRequirement(document =>
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
        });

        return builder;
    }

    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        return builder;
    }

    public static WebApplicationBuilder AddErrorHandling(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        });
        builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        return builder;
    }

    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Resend"));
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IShipmentRequestService, ShipmentRequestService>();
        builder.Services.AddScoped<IShipmentService, ShipmentService>();
        builder.Services.AddScoped<IVesselService, VesselService>();
        builder.Services.AddScoped<IOfferService, OfferService>();
        builder.Services.AddScoped<IRatingService, RatingService>();
        builder.Services.AddScoped<ICarrierRouteService, CarrierRouteService>();
        builder.Services.AddScoped<INileBerthService, NileBerthService>();
        builder.Services.AddScoped<INotificationService, NotificationService>();
        builder.Services.AddScoped<IProfileService, ProfileService>();
        builder.Services.AddScoped<ICarrierService, CarrierService>();
        builder.Services.AddScoped<ShipmentRequestMapper>();
        builder.Services.AddScoped<ShipmentMapper>();
        builder.Services.AddScoped<OfferMapper>();
        builder.Services.AddScoped<NileBerthMapper>();
        return builder;
    }


    public static WebApplicationBuilder AddFilesUploadServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<SupabaseStorageSettings>()
            .Bind(builder.Configuration.GetSection("Supabase"))
            .Validate(o => Uri.TryCreate(o.Url, UriKind.Absolute, out _)
                           && !string.IsNullOrWhiteSpace(o.ServiceKey),
                "Supabase:Url and Supabase:ServiceKey are required")
            .ValidateOnStart();

        builder.Services.AddHttpClient<IFileStorage, SupabaseStorage>((sp, http) =>
        {
            var s = sp.GetRequiredService<IOptions<SupabaseStorageSettings>>().Value;
            http.BaseAddress = new Uri(s.BaseUrl + "/");
            http.DefaultRequestHeaders.Add("apikey", s.ServiceKey);
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", s.ServiceKey);
            http.Timeout = TimeSpan.FromSeconds(30);
        });
        return builder;
    }
    
    
    public static WebApplicationBuilder AddNotificationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSignalR();
        builder.Services.AddSingleton<IUserIdProvider, NameIdentifierUserIdProvider>();
        builder.Services.AddOptions();
        builder.Services.AddHttpClient<ResendClient>();
        builder.Services.Configure<ResendClientOptions>(o => { o.ApiToken = builder.Configuration["Resend:ApiKey"]!; });
        builder.Services.AddTransient<IResend, ResendClient>();

        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Resend"));
        builder.Services.Configure<AppUrls>(builder.Configuration.GetSection("App"));
        builder.Services.AddOptions<AppUrls>()
            .Bind(builder.Configuration.GetSection("App"))
            .Validate(o => Uri.TryCreate(o.BaseUrl, UriKind.Absolute, out var uri)
                           && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps),
                "App:BaseUrl must not be empty and valid")
            .ValidateOnStart();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddSingleton<MatchNotificationQueue>();
        builder.Services.AddHostedService<MatchNotificationWorker>();
        return builder;
    }

    public static WebApplicationBuilder AddAuthenticationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 8; //
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredUniqueChars = 1;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
                options.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<EmailConfirmationTokenProvider<User>>("emailconfirmation");
        builder.Services.Configure<DataProtectionTokenProviderOptions>(o =>
            o.TokenLifespan = TimeSpan.FromHours(2));

        builder.Services.Configure<EmailConfirmationTokenProviderOptions>(o =>
            o.TokenLifespan = TimeSpan.FromDays(3));

        builder.Services.AddMemoryCache();
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddDataProtection()
            .PersistKeysToDbContext<ApplicationDbContext>()
            .SetApplicationName("RiverLine");

        return builder;
    }

    public static WebApplicationBuilder AddRateLimiting(this WebApplicationBuilder builder)
    {
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.ContentType = "text/plain";
                await context.HttpContext.Response.WriteAsync("rate_limited", cancellationToken);
            };

            options.AddPolicy(RateLimitPolicies.Public, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 60,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));

            options.AddPolicy(RateLimitPolicies.Sensitive, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                                  ?? httpContext.Connection.RemoteIpAddress?.ToString()
                                  ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(15),
                        QueueLimit = 0
                    }));
        });

        return builder;
    }

    public static WebApplicationBuilder AddCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("RiverLinePolicy", policy =>
            {
                var origins = builder.Configuration
                    .GetSection("Cors:AllowedOrigins")
                    .Get<string[]>() ?? [];

                policy
                    .WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        return builder;
    }
}