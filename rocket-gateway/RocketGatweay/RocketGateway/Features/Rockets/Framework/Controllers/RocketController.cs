using Microsoft.AspNetCore.Mvc;
using RocketGateway.Features.Rockets.Core.Models.Events;
using RocketGateway.Features.Rockets.Core.Services.Interfaces;
using RocketGateway.Features.Rockets.Framework.ExternalModels.Inbound.Events;
using RocketGateway.Features.Rockets.Framework.ExternalModels.Outbound;
using RocketGateway.Features.Shared.Mapping.Interfaces;
using RocketGateway.Features.Shared.Models;

namespace RocketGateway.Features.Rockets.Framework.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RocketController : ControllerBase
{
    private readonly IRocketsService rocketsService;
    private readonly IMapper<RocketChangeEvent, OperationResult<RocketChangeCoreEvent, ErrorModel>> inputModelMapper;
    public RocketController(
        IRocketsService rocketsService, 
        IMapper<RocketChangeEvent, OperationResult<RocketChangeCoreEvent, ErrorModel>> inputModelMapper
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