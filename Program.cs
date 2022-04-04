using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using R10.models;
using R10.Security.Requirements;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
// var connectionString = builder.Configuration.GetConnectionString("MyBlogContextConnection");
// builder.Services.AddDbContext<MyBlogContext>(options =>
//     options.UseSqlServer(connectionString));builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
//     .AddEntityFrameworkStores<MyBlogContext>();
// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<MyBlogContext>(options => 
       options.UseSqlServer(builder.Configuration.GetConnectionString("MyLogContext")));

builder.Services.Configure<MailSettings>(
    builder.Configuration.GetSection("MailSetting")
);

builder.Services.AddSingleton<IEmailSender, SendMailService>();

// trang login tùy biến
builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<MyBlogContext>()
                .AddDefaultTokenProviders();
//trang mặc dịnh trong code
// builder.Services.AddDefaultIdentity<AppUser>()
//                 .AddEntityFrameworkStores<MyBlogContext>()
//                 .AddDefaultTokenProviders();


//
builder.Services.ConfigureApplicationCookie(option =>{
    option.LoginPath = "/Login";
    option.LogoutPath = "/Logout";
    option.AccessDeniedPath = "/AccessDenied";
    // option.LoginPath = "/Login/";
    // option.LogoutPath = "/Logout/";
    // option.AccessDeniedPath = "/kotruycap.html";
});

// Truy cập IdentityOptions
builder.Services.Configure<IdentityOptions> (options => {
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds (10); // Khóa ...
    options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 5 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
    options.SignIn.RequireConfirmedAccount = true;          // Cần xác để đăng nhập

});

builder.Services.AddAuthentication()
                    .AddGoogle(option =>{
                        var gconfic = builder.Configuration.GetSection("Authentication:Google");
                        option.ClientId = gconfic["ClientId"];
                        option.ClientSecret =gconfic["ClientSecret"];
                        //mặc định callback là: /sigin-google 
                        option.CallbackPath = "/LoginByGoogle";
                    })
                    .AddFacebook(option =>{
                        var fconfic = builder.Configuration.GetSection("Authentication:FaceBook");
                        option.AppId = fconfic["AppId"];
                        option.AppSecret =fconfic["AppSecret"];
                        
                        option.CallbackPath = "/LoginByFaceBook";
                    });


// Role service

builder.Services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();

// Policy authorization
builder.Services.AddAuthorization(option =>{
    // option.AddPolicy("P1", policyBuilder =>{
    //     //Đk policy
    // });
    option.AddPolicy("AllowEditRole", policyBuilder =>{
        //Đk policy
        policyBuilder.RequireAuthenticatedUser();
        
        // policyBuilder.RequireRole("Admin");

        // policyBuilder.RequireClaim("tenclaim", "c1","c2");
        // policyBuilder.RequireClaim("tenclaim", new string[] {
        //     "c1",
        //     "c2"
        // });

        // policyBuilder.RequireClaim("managa.role", "add", "update");
        // policyBuilder.RequireClaim("canedit", "post", "update");
        // policyBuilder.RequireClaim("canedit", new string[] {
        //     "post",
        //     "update"
        // });
        
    });

    option.AddPolicy("InGenZ", policyBuilder =>{
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.Requirements.Add(new GenZRequirement()); // tạo Requirement ở Security

        //tạo dịch vụ để xét requirement là authorization handler
    });
    option.AddPolicy("ShowAdminMenu", policyBuilder =>{
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.RequireRole("Admin");
    });
    option.AddPolicy("CanUpdateArticle", policyBuilder =>{
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.Requirements.Add(new ArticleUpdateRequirement());
    });
});

//Đăng kí dịch vụ authorization handler
builder.Services.AddTransient<IAuthorizationHandler, AppAuthorizationHandler>();

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
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
