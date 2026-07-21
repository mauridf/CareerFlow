using CareerFlow.Application.Common.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CareerFlow.Api.Helpers;

public static class ResponseHelper
{
    public static OkObjectResult OkResponse<T>(T data, HttpContext? httpContext = null, MetaResponse? meta = null)
    {
        meta ??= new MetaResponse();
        meta.Timestamp = DateTime.UtcNow;
        meta.RequestId ??= httpContext?.TraceIdentifier;

        return new OkObjectResult(new SuccessResponse<T>
        {
            Success = true,
            Data = data,
            Meta = meta
        });
    }

    public static CreatedResult CreatedResponse<T>(string uri, T data, HttpContext? httpContext = null)
    {
        return new CreatedResult(uri, new SuccessResponse<T>
        {
            Success = true,
            Data = data,
            Meta = new MetaResponse
            {
                Timestamp = DateTime.UtcNow,
                RequestId = httpContext?.TraceIdentifier
            }
        });
    }

    public static ObjectResult PaginatedResponse<T>(
        T data,
        int page,
        int perPage,
        int total,
        HttpContext? httpContext = null)
    {
        var totalPages = (int)Math.Ceiling(total / (double)perPage);

        return new OkObjectResult(new SuccessResponse<T>
        {
            Success = true,
            Data = data,
            Meta = new MetaResponse
            {
                Timestamp = DateTime.UtcNow,
                RequestId = httpContext?.TraceIdentifier,
                Page = page,
                PerPage = perPage,
                Total = total,
                TotalPages = totalPages
            }
        });
    }

    public static OkObjectResult MessageResponse(string message, HttpContext? httpContext = null)
    {
        return OkResponse(new { message }, httpContext);
    }
}
