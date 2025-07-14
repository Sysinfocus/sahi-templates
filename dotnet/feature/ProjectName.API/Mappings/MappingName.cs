public static class {{Model}}Mappings
{
    public static {{Model}}Dto ToDTO(this {{Model}} model)
    {
        return new() {
            {{ModelProperties}}
        };
    }

    public static {{Model}} ToModel(this {{Model}}Dto dto, {{IDType}}? id = null)
    {
        return new(
            id ?? new(),
            {{!DTOProperties}}
        );
    }
}