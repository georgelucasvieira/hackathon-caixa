using System.ComponentModel.DataAnnotations;

namespace API_Simulacao.DTOs;
public class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var dto = context.Arguments.OfType<T>().FirstOrDefault();

        if (dto is null)
            return Results.BadRequest("Payload inválido.");

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(dto);

        if (!Validator.TryValidateObject(dto, validationContext, validationResults, validateAllProperties: true))
        {
            var erros = validationResults.Select(v => new
            {
                Campo = v.MemberNames.FirstOrDefault() ?? "",
                Erro = v.ErrorMessage ?? "Erro de validação"
            });

            return Results.BadRequest(erros);
        }

        return await next(context);
    }
}
