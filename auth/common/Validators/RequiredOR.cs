using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AuthCommon.Validators;

public class RequiredORAttribute : ValidationAttribute
{
    private readonly PropertyInfo another;
    private readonly string? errorMsg;

    public RequiredORAttribute(Type objectType, string memberName)
    {
        this.another = objectType.GetProperty(memberName) 
            ?? throw new ArgumentException($"Can't find property with name: {memberName}");
    }

    public RequiredORAttribute(Type objectType, string memberName, string errorMsg) :
        this(objectType, memberName)
    {
        this.errorMsg = errorMsg;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value != null || another.GetValue(context.ObjectInstance) != null)
            return ValidationResult.Success;

        return new ValidationResult(errorMsg ?? $"{context.MemberName} or {another.Name} has to be setted!");
    }
}