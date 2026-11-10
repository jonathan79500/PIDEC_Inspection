using Inspection.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace Inspection.Api.Filters;   // use your own namespace

public class ApiExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case NotFoundException ex:
                Set(context, StatusCodes.Status404NotFound, ex.Message);
                break;
            case BusinessRuleException ex:
                Set(context, StatusCodes.Status400BadRequest, ex.Message);
                break;
        }
    }

    private static void Set(ExceptionContext context, int status, string message)
    {
        context.Result = new ObjectResult(new { error = message }) { StatusCode = status };
        context.ExceptionHandled = true;
    }
}