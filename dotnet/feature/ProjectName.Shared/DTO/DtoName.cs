namespace ProjectName.Shared.DTO;
public sealed class {{DTO}}Dto : ModelValidator
{
    {{DtoProps}}

    public override Dictionary<string, string>? Errors()
    {
        var errors = new Dictionary<string, string>();
        {{Validations}}
        return errors?.Count > 0 ? errors : null;
    }
}