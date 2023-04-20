using System.Text;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BackendApi.Filters;

public class EnumSchemaFilter : ISchemaFilter
{
    public void ApplyOld(OpenApiSchema model, SchemaFilterContext context)
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

    public void Applydf(OpenApiSchema model, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            var fields = context.Type.GetFields();
            var values = Enum.GetValues(context.Type);

            var names = new OpenApiArray();
            for (int i = 1; i < fields.Length; i++)
            {
                var enumValue = (int)values.GetValue(i-1);
                names.Add(new OpenApiString(fields[i].Name));
            }

            // model.Enum.Clear();
            model.Extensions.Add(
                "x-enumNames",
                names
            );



            // values.GetValue(1);
            model.Example = new OpenApiInteger((int)values.GetValue(0));
        }
    }

    public void Apply(OpenApiSchema model, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            var fields = context.Type.GetFields();
            var values = Enum.GetValues(context.Type);
            var sb = new StringBuilder();
            for (int i = 1; i < fields.Length; i++)
            {
                // var enumValue = ;
                // names.Add(new OpenApiString());
                sb.Append($"{fields[i].Name} = {(int)values.GetValue(i-1)}\r\n");
            }

            model.Description = sb.ToString();
            // values.GetValue(1);
            model.Example = new OpenApiInteger((int)values.GetValue(0));
        }
    }
}