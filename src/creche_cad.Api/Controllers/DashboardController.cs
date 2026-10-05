using creche_cad.Service.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace creche_cad.Controllers;
[ApiController,Route("api/dashboard")]
public class DashboardController(ISender sender,IConfiguration config):ControllerBase {
 [HttpGet] public async Task<IActionResult> Get(CancellationToken cancellationToken) =>
  Ok(await sender.Send(new GetDashboardSummaryQuery(config.GetValue<bool>("Demo:Enabled")), cancellationToken));
}
