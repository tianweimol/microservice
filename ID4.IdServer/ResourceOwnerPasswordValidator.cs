﻿using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ID4.IdServer
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            //根据context.UserName和context.Password与数据库的数据做校验，判断是否合法
            if (context.UserName == "mol" && context.Password == "111")
            {
                context.Result = new GrantValidationResult(
                subject: context.UserName,
                authenticationMethod: "custom",
                claims: new Claim[] { new Claim("Name", context.UserName), new Claim("UserId", "111"), new Claim("RealName", "it"), new Claim("Email", "it@tiens.com") });
            }
            else
            {
                //验证失败
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid custom credential");
            }
        }
    }
}
