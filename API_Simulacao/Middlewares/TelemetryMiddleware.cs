namespace API_Simulacao.Middlewares
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using API_Simulacao.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class TelemetryMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TelemetryMiddleware> _logger;

        public TelemetryMiddleware(RequestDelegate next, ILogger<TelemetryMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, TelemetriaRepository telemetriaRepo)
        {
            var stopwatch = Stopwatch.StartNew();
            await _next(context);
            stopwatch.Stop();

            var method = context.Request.Method;
            var path = context.Request.Path;
            var statusCode = context.Response.StatusCode;

            var metric = new MetricasTelemetria
            {
                NomeApi = context.Request.Path,
                TempoMs = (int)stopwatch.ElapsedMilliseconds,
                StatusCode = context.Response.StatusCode,
                Data = DateTime.Now
            };

            await telemetriaRepo.GravarMetricaAsync(metric);

            _logger.LogInformation("HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms",
                method, path, statusCode, stopwatch.ElapsedMilliseconds);
        }
    }
}