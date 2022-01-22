using Microsoft.AspNetCore.Mvc;
using Todo.Application.Common.AppRequests;

namespace Todo.WebApi.Common;

public static class ResultBuilder
{
    public static IResult Build(AppResponse appResponse)
    {
        return appResponse.StatusCode switch
        {
            200 => Results.Ok(appResponse),
            400 => Results.BadRequest(appResponse),
            401 => Results.Unauthorized(),
            403 => Results.Forbid(),
            404 => Results.NotFound(appResponse),
            _ => Results.Json(appResponse, statusCode: appResponse.StatusCode),
        };
    }
}
