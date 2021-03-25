# 1 前置阅读
在阅读本文章之前，你可以先阅读：
* IdentityServer4是什么
* IdentityServer4：客户端凭据模式

# 2 授权认证服务（IdentityServer）
API资源配置，中间件配置与客户端凭据模式保持一致。

**首先**，增加用户，像API资源（也称为 Scope）、客户端一样，用户也有一个基于内存存储（In-Memory）的实现。
```
public static List<TestUser> Users
{
    get
    {
        return new List<TestUser>
        {
            new TestUser
            {
                SubjectId = "818727",
                Username = "alice",
                Password = "alice"
            },
            new TestUser
            {
                SubjectId = "88421113",
                Username = "bob",
                Password = "bob"
            }
        };
    }
}
```

**然后**，定义客户，即用于访问新API的客户端应用程序。
```
public static IEnumerable<Client> Clients =>
    new List<Client>
    {
        new Client
        {
            // 定义客户端ID
            ClientId = "client2",
            // 授权方式为用户密码模式授权，类型可参考GrantTypes枚举
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            // 定义客户端秘钥
            ClientSecrets =
            {
                new Secret("secret2".Sha256())
            },
            // 允许客户端访问的范围
            AllowedScopes = { "api1" }
        }
    };
```

**最后**，配置IdentityServer，在Startup.ConfigureServices中，如下所示：
```
public void ConfigureServices(IServiceCollection services)
{
    var builder = services.AddIdentityServer()
        .AddInMemoryApiScopes(Config.ApiScopes)
        .AddInMemoryClients(Config.Clients)
        .AddTestUsers(Config.Users);

    builder.AddDeveloperSigningCredential();
}
```

# 3 测试
用PostMan获取Token 如图所示：
![image](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210322/20210324153012.png)
