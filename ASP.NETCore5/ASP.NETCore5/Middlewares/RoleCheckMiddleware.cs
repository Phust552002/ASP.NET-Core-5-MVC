//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using ASP.NETCore5.Middlewares;

//namespace ASP.NETCore5.Middlewares
//{
//    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
//    public class RoleCheckMiddleware
//    {
//        private readonly RequestDelegate _next;

//        public RoleCheckMiddleware(RequestDelegate next)
//        {
//            _next = next;
//        }

//        public async Task InvokeAsync(HttpContext context)
//        {
//            //var path = context.Request.Path.Value?.ToLower() ?? "";
//            //var roles = context.Session.GetString("UserRoles");
//            //var username = context.Session.GetString("UserName");

//            //Console.WriteLine($"👉 [Middleware] Path: {path}");
//            //Console.WriteLine($"👉 [Middleware] User: {username ?? "(null)"}");
//            //Console.WriteLine($"👉 [Middleware] Role: {roles ?? "(null)"}");
//            //// ✅ Bỏ qua static files, login, accessdenied, api...
//            //if (path.StartsWith("/css") ||
//            //    path.StartsWith("/js") ||
//            //    path.StartsWith("/lib") ||
//            //    path.StartsWith("/images") ||
//            //    path.StartsWith("/account/login") ||
//            //    path.StartsWith("/account/register") ||
//            //    path.StartsWith("/accessdenied"))
//            //{
//            //    await _next(context);
//            //    return;
//            //}

//            //// ✅ Chỉ kiểm tra quyền khi vào /admin
//            //if (path.StartsWith("/admin"))
//            //{
//            //    if (string.IsNullOrEmpty(roles))
//            //    {
//            //        //context.Response.Redirect("/admin/list");
//            //        return;
//            //    }

//            //    if (!(roles.Contains("Admin") || roles.Contains("Manager")))
//            //    {
//            //        context.Response.Redirect("/Account/AccessDenied");
//            //        return;
//            //    }
//            //}


//            await _next(context);
//        }
//    }

//    // Extension method used to add the middleware to the HTTP request pipeline.
//    public static class RoleCheckMiddlewareExtensions
//    {
//        public static IApplicationBuilder UseRoleCheckMiddleware(this IApplicationBuilder builder)
//        {
//            return builder.UseMiddleware<RoleCheckMiddleware>();
//        }
//    }
//}
