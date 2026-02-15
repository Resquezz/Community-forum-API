using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.API.Swagger
{
    public class AuthorizeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAllowAnonymous = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AllowAnonymousAttribute>()
                .Any()
                || context.MethodInfo.DeclaringType?
                    .GetCustomAttributes(true)
                    .OfType<AllowAnonymousAttribute>()
                    .Any() == true;

            if (hasAllowAnonymous)
            {
                operation.Security?.Clear();
                return;
            }

            var hasAuthorize = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Any()
                || context.MethodInfo.DeclaringType?
                    .GetCustomAttributes(true)
                    .OfType<AuthorizeAttribute>()
                    .Any() == true;

            if (!hasAuthorize)
            {
                operation.Security?.Clear();
            }
        }
    }
}
