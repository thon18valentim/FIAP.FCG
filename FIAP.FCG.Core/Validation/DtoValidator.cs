using System.ComponentModel.DataAnnotations;

namespace FIAP.FCG.Core.Validation;

public static class DtoValidator
{
    public static void ValidateObject(object instance)
    {
        var ctx = new ValidationContext(instance);
        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(instance, ctx, results, validateAllProperties: true))
        {
            var msg = string.Join("; ", results.Select(r => r.ErrorMessage));
            throw new ValidationException(msg);
        }
    }
}
