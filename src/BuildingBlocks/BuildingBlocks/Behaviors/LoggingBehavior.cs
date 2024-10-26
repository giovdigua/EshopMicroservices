using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Behaviors;

public class LoggingBehavior<TRequest, TResponse>
    (ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("[START] Handle request={Request} - Response={Response} - RequestData={RequestData}",
            typeof(TRequest).Name, typeof(TResponse).Name, request);

        var timer = new Stopwatch();
        timer.Start();

        var response = await next();

        timer.Stop();  
        var timerTaken = timer.Elapsed;
        if (timerTaken.Seconds > 3) // if the rquest is greater than 3 second, then log
        {
            logger.LogWarning("[PERFORMANCE] The request {Request} took {TimeTaken} seconds",
                typeof(TRequest).Name, timerTaken.Seconds);
        }

        logger.LogInformation("[END] Handle request={Request} - Response={Response}", typeof(TRequest).Name, typeof(TResponse).Name);
        return response;
    }
}
