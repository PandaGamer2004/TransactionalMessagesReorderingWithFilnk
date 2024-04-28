using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RocketGateway.Extensions;
using RocketGateway.Features.Rockets.Core.Models;
using RocketGateway.Features.Rockets.Core.Models.Events;
using RocketGateway.Features.Rockets.Core.Models.Messages;
using RocketGateway.Features.Rockets.Core.Services.Interfaces;
using RocketGateway.Features.Rockets.Models;
using RocketGateway.Features.Rockets.Models.Events;
using RocketGateway.Features.Shared.Mapping.Interfaces;
using RocketGateway.Features.Shared.Models;

namespace RocketGateway.Features.Rockets.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RocketController : ControllerBase
{
    private readonly IRocketsService rocketsService;
    private readonly IMapper<RocketChangeEvent, OperationResult<RocketChangeDomainEvent, ErrorModel>> inputModelMapper;
    public RocketController(
        IRocketsService rocketsService, 
        IMapper<RocketChangeEvent, OperationResult<RocketChangeDomainEvent, ErrorModel>> inputModelMapper
        )
    {
        this.rocketsService = rocketsService;
        this.inputModelMapper = inputModelMapper;
    }
    

    [HttpPost("patch")]
    public async ValueTask<IActionResult> PatchRocket(RocketChangeEvent rocketChangeEvent)
    {
        var mappingResult = inputModelMapper.Map(rocketChangeEvent);
        if (mappingResult.IsSuccess)
        {
            var result = 
                await rocketsService.ProcessUnfilteredEvent(mappingResult.SuccessModel);
            if (result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(new ErrorModel
            {
                ErrorCode = ErrorCodes.FailedToProcessEvent,
                ErrorDescription = result.ErrorModel
            });
        }
        return BadRequest(mappingResult.ErrorModel);
    }
    
}