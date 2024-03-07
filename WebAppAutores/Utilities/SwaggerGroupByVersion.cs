using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebAppAutores.Utilities
{
    public class SwaggerGroupByVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var namespaceController = controller.ControllerType.Namespace; // Controllers.v1
            var versionAPI = namespaceController.Split(".").Last().ToLower(); // v1
            controller.ApiExplorer.GroupName = versionAPI; // assign it
        }
    }
}
