using MassTransit.Initializers;
using Microsoft.AspNetCore.Mvc;
using RocketGateway.Features.Rockets.Core.Models;
using RocketGateway.Features.Rockets.Core.Models.Events;
using RocketGateway.Features.Rockets.Core.Services.Interfaces;
using RocketGateway.Features.Rockets.Core.State;
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
    private readonly IMapper<RocketAggregateState, ResponseRocketModel> aggregateStateMapper;
    public RocketController(
        IRocketsService rocketsService, 
        IMapper<RocketChangeEvent, OperationResult<RocketChangeCoreEvent, ErrorModel>> inputModelMapper, 
        IMapper<RocketAggregateState, ResponseRocketModel> aggregateStateMapper
        )
    {
        this.rocketsService = rocketsService;
        this.inputModelMapper = inputModelMapper;
        this.aggregateStateMapper = aggregateStateMapper;
    }


    [HttpGet("{id}")]
    public async ValueTask<IActionResult> GetRocketById(Guid rocketId)
    {
        var rocketAccessResults 
            = await rocketsService.GetRocketById(RocketId.FromValue(rocketId));
        if (rocketAccessResults.IsSuccess)
        {
            var projectedState
                = aggregateStateMapper.Map(rocketAccessResults.SuccessModel.State);
            return Ok(projectedState);
        }

        return BadRequest(new ErrorModel
        {
            ErrorCode = ErrorCodes.FailureToAccessRocketById,
            ErrorDescription = rocketAccessResults.ErrorModel
        });
    }
    
    
    [HttpGet]
    public async Task<IActionResult> GetRockets()
    {
        var rocketsAccessResult = await rocketsService.GetRockets();
        if (rocketsAccessResult.IsSuccess)
        {
            return Ok(rocketsAccessResult
                .SuccessModel
                .Select(model => model.State)
                .Select(aggregateStateMapper.Map)
            );
        }

        return BadRequest(new ErrorModel
        {
            ErrorCode = ErrorCodes.FailedToLoadRockets,
            ErrorDescription = rocketsAccessResult.ErrorModel
        });
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