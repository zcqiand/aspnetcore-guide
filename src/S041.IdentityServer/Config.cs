using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace S041.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
            new ApiScope("api1", "My API")
            };

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
    }
}
