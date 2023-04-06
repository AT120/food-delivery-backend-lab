using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BackendApi.FIlters;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema model, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            var fields = context.Type.GetFields();
            var values = Enum.GetValues(context.Type);

            model.Enum.Clear();
            for (int i = 1; i < fields.Length; i++)
            {
                model.Enum.Add(new OpenApiString($"{fields[i].Name} = {(int)values.GetValue(i-1)}"));
            }
            // values.GetValue(1);
            model.Example = new OpenApiInteger((int)values.GetValue(0));
        }
    }
}