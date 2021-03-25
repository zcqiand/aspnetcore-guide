using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;

namespace S043.IdentityServer
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
    }
}
