# 1 什么是IdentityServer4
IdentityServer4是用于ASP.NET Core的OpenID Connect和OAuth 2.0框架。

# 2 使用客户端凭据保护API
## IdentityServer 设置
**首先**，创建S041.IdentityServer Web API 项目。

**然后**，定义API范围，API是您系统中要保护的资源，给您的API取一个逻辑名称就很重要。开发人员将使用它通过您的身份服务器连接到您的api。它应该以简单的方式向开发人员和用户描述您的api。
```
public static class Config
{
    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("api1", "My API")
        };
}
```

**接着**，定义客户，即用于访问新API的客户端应用程序。
```
public static IEnumerable<Client> Clients =>
    new List<Client>
    {
        new Client
        {
            // 定义客户端ID
            ClientId = "client",

            // 授权方式为用户密码模式授权，类型可参考GrantTypes枚举
            AllowedGrantTypes = GrantTypes.ClientCredentials,

            // 定义客户端秘钥
            ClientSecrets =
            {
                new Secret("secret".Sha256())
            },

            // 允许客户端访问的范围
            AllowedScopes = { "api1" }
        }
    };
```
您可以将ClientId和ClientSecret视为应用程序本身的登录名和密码。

**接着**，配置IdentityServer，在Startup.ConfigureServices中，如下所示：
```
public void ConfigureServices(IServiceCollection services)
{
    var builder = services.AddIdentityServer()
        .AddInMemoryApiScopes(Config.ApiScopes)
        .AddInMemoryClients(Config.Clients);

    builder.AddDeveloperSigningCredential();
}
```

**最后**，配置中间件，在Startup.Configure中，如下所示：
```
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseDeveloperExceptionPage();

    app.UseIdentityServer();
}
```

如果运行服务器并将浏览器导航到https://localhost:5001/.well-known/openid-configuration，则应该看到所谓的发现文档。

![image](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210322/1_discovery.png)

首次启动时，IdentityServer将为您创建一个开发人员签名密钥，该文件名为tempkey.jwk。您无需将该文件签入源代码管理中，如果不存在该文件将被重新创建。

## API 设置
**首先**，将Microsoft.AspNetCore.Authentication.JwtBearer的NuGet软件包安装到您的应用程序中。
```
Microsoft.AspNetCore.Authentication.JwtBearer
```

**然后**，配置API，在Startup.ConfigureServices中，如下所示：
```
public void ConfigureServices(IServiceCollection services)
{

    services.AddControllers();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "S041.Api", Version = "v1" });
    });

    services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
            options.Authority = "https://localhost:5001";

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false
            };
        });
}
```
AddAuthentication将身份验证服务添加到DI并配置Bearer为默认方案。

**接着**，配置中间件，在Startup.Configure中，如下所示：
```
public void Configure(IApplicationBuilder app)
{
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```
* UseAuthentication 将身份验证中间件添加到管道中，以便对主机的每次调用都将自动执行身份验证。

**最后**，修改控制器，添加 [Authorize]
```
[ApiController]
[Route("[controller]")]
[Authorize]
public class WeatherForecastController : ControllerBase
{
}
```

## API 测试
**首先**，用PostMan测试未加Token的API, 如图所示：
![image](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210322/20210322110914.png)

**然后**，用PostMan获取Token 如图所示：
![image](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210322/20210322111806.png)

**最后**，用PostMan测试加Token的API 如图所示：
![image](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210322/20210322111954.png)

## API 授权
目前，API接受您的身份服务器发出的所有访问令牌。

**首先**，我们将添加代码，该代码允许检查客户端请求（并被授予）的访问令牌中是否存在作用域。
```
services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "api1");
    });
});
```

**最后**，为路由系统中的所有API端点设置策略：
```
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers()
        .RequireAuthorization("ApiScope");
});
```

# 3 带有ASP.NET Core的交互式应用程序
## 创建一个MVC客户端
**首先**，创建S042.MvcClient Web MVC 项目。

**然后**，将Microsoft.AspNetCore.Authentication.OpenIdConnect的NuGet软件包安装到您的应用程序中。
```
Microsoft.AspNetCore.Authentication.OpenIdConnect
```

**接着**，以下内容添加到ConfigureServices中Startup：
```
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllersWithViews();

    JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

    services.AddAuthentication(options =>
    {
        options.DefaultScheme = "Cookies";
        options.DefaultChallengeScheme = "oidc";
    })
    .AddCookie("Cookies")
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = "https://localhost:5001";

        options.ClientId = "mvc";
        options.ClientSecret = "secret";
        options.ResponseType = "code";

        options.SaveTokens = true;

        options.Scope.Add("api1");
        options.Scope.Add("offline_access");
    });
}
```
AddAuthentication 将身份验证服务添加到DI。

我们使用一个Cookie在本地登录的用户（通过"Cookies"为DefaultScheme），和我们设定的DefaultChallengeScheme到oidc，因为当我们需要用户登录，我们将使用ID连接协议。

然后AddCookie，我们使用添加可以处理cookie的处理程序。

最后，AddOpenIdConnect用于配置执行OpenID Connect协议的处理程序。该Authority指示了信任令牌服务所在。然后，我们通过ClientId和标识此客户ClientSecret。 SaveTokens用于将来自IdentityServer的令牌保留在cookie中。

**最后**，添加UseAuthentication到Configure在Startup：
```
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute()
        .RequireAuthorization();
});
```
RequireAuthorization方法禁用整个应用程序的匿名访问。

## 添加对OpenID Connect Identity Scope的支持
与OAuth 2.0相似，OpenID Connect也使用范围概念。同样，范围代表您要保护的内容以及客户端要访问的内容。与OAuth相比，OIDC中的范围不代表API，而是诸如用户ID，名称或电子邮件地址之类的身份数据。

**首先**，通过修改以下属性来添加对标准openid（主题ID）和profile（名字，姓氏等）范围的支持：IdentityResourcesConfig.cs
```
public static IEnumerable<IdentityResource> IdentityResources =>
    new List<IdentityResource>
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
    };
```

**最后**，在startup.cs以下位置向IdentityServer注册身份资源：
```
var builder = services.AddIdentityServer()
    .AddInMemoryIdentityResources(Config.IdentityResources)
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddInMemoryClients(Config.Clients);
```

## 添加测试用户
示例UI还带有一个内存中的“用户数据库”。您可以通过添加AddTestUsers扩展方法在IdentityServer中启用它：
```
var builder = services.AddIdentityServer()
    .AddInMemoryIdentityResources(Config.IdentityResources)
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddInMemoryClients(Config.Clients)
    .AddTestUsers(TestUsers.Users);
```
当您导航到TestUsers类，你可以看到两个用户名为alice和bob以及一些身份声明定义。您可以使用这些用户登录。

## 将MVC客户端添加到IdentityServer配置
基于OpenID Connect的客户端与到目前为止我们添加的OAuth 2.0客户端非常相似。但是，由于OIDC中的流始终是交互式的，因此我们需要向我们的配置中添加一些重定向URL。
```
public static IEnumerable<Client> Clients =>
    new List<Client>
    {
        // machine to machine client (from quickstart 1)
        new Client
        {
            ClientId = "client",
            ClientSecrets = { new Secret("secret".Sha256()) },

            AllowedGrantTypes = GrantTypes.ClientCredentials,
            // scopes that client has access to
            AllowedScopes = { "api1" }
        },
        // interactive ASP.NET Core MVC client
        new Client
        {
            ClientId = "mvc",
            ClientSecrets = { new Secret("secret".Sha256()) },

            AllowedGrantTypes = GrantTypes.Code,

            // where to redirect to after login
            RedirectUris = { "https://localhost:5002/signin-oidc" },

            // where to redirect to after logout
            PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

            AllowedScopes = new List<string>
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile
            }
        }
    };
```

## 测试客户端

