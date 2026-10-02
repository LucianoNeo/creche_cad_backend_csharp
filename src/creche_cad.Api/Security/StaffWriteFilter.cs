using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace creche_cad.Api.Security;
public class StaffWriteFilter:IAsyncAuthorizationFilter {
 public Task OnAuthorizationAsync(AuthorizationFilterContext context) {
  var request=context.HttpContext.Request;
  if(context.HttpContext.User.IsInRole("Viewer") && !HttpMethods.IsGet(request.Method) && !request.Path.StartsWithSegments("/api/auth"))
   context.Result=new ForbidResult();
  return Task.CompletedTask;
 }
}
