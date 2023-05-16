using AdminPanel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AdminPanel.Filters;

public class ModelValidationFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {

        if (!context.ModelState.IsValid)
        {
            var controller = context.Controller as Controller;
            var errors = context.ModelState
                    .Where(m => m.Value.Errors.Count > 0)
                    .SelectMany(m => 
                        m.Value.Errors.Select(e => e.ErrorMessage)
                    );

            var evm = new ErrorViewModel
            {
                Message = string.Join(';', errors)
            };

            context.Result = controller.View("Error", evm);
        }

        base.OnActionExecuting(context);
        
    }
}
