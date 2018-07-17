using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ID4.IdServer
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            List<ApiResource> resources = new List<ApiResource>();
            //ApiResource第一个参数是应用的名字，第二个参数是描述
            resources.Add(new ApiResource("缓存服务", "缓存API"));
            resources.Add(new ApiResource("订单服务名", "订单API"));
            return resources;
        }
        /// <summary>
        /// 返回账号列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            List<Client> clients = new List<Client>();
            clients.Add(new Client
            {
                ClientId = "clientPC1",//API账号、客户端Id
                // AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets =
                    {
                    new Secret("a1".Sha256())//秘钥
                    },
                AllowedScopes = { "缓存服务", "订单服务名" , IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile}//这个账号支持访问哪些应用
            });


            clients.Add(new Client
            {
                ClientId = "clientAndroid1",//API账号、客户端Id
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                    {
                    new Secret("a2".Sha256())//秘钥
                    },
                AllowedScopes = { "缓存服务" }//这个账号支持访问哪些应用
            });

            clients.Add(new Client
            {
                ClientId = "clientIOS1",//API账号、客户端Id
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                    {
                    new Secret("a3".Sha256())//秘钥
                    },
                AllowedScopes = { "订单服务名" }//这个账号支持访问哪些应用
            });


            return clients;
        }
    }
}
