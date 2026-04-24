using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

public static class ControllerExtensions
{
    public static async Task<string> RenderViewAsync<TModel>(
        this Controller controller,
        string viewName,
        TModel model,
        bool partial = false)
    {
        controller.ViewData.Model = model;

        using var writer = new StringWriter();
        var viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
        var viewResult = viewEngine.FindView(controller.ControllerContext, viewName, !partial);

        if (!viewResult.Success)
            throw new InvalidOperationException($"No se encontró la vista {viewName}");

        var viewContext = new ViewContext(
            controller.ControllerContext,
            viewResult.View,
            controller.ViewData,
            controller.TempData,
            writer,
            new HtmlHelperOptions() // 👉 ahora sí con el namespace correcto
        );

        await viewResult.View.RenderAsync(viewContext);
        return writer.GetStringBuilder().ToString();
    }
}
