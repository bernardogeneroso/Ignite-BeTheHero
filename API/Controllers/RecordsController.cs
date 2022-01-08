using Application.Records;
using Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class RecordsController : BaseApiController
{
  [HttpGet]
  public async Task<IActionResult> GetRecords()
  {
    return HandleResult(await Mediator.Send(new List.Query()));
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetRecord(string id)
  {
    return HandleResult(await Mediator.Send(new Details.Query { Id = id }));
  }

  [HttpPost]
  public async Task<IActionResult> CreateRecord(Record record)
  {
    return HandleResult(await Mediator.Send(new Create.Command { Record = record }));
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> EditRecord(string id, Record record)
  {
    return HandleResult(await Mediator.Send(new Edit.Command { Id = id, Record = record }));
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteRecord(string id)
  {
    return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
  }
}