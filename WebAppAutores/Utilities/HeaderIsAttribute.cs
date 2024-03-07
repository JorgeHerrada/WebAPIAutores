using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace WebAppAutores.Utilities
{
    public class HeaderIsAttribute : Attribute, IActionConstraint // will allow us to choose the endpoint to execute
    {
        private readonly string header;
        private readonly string value;

        public HeaderIsAttribute(string header, string value)
        {
            this.header = header;
            this.value = value;
        }

        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {
            var headers = context.RouteContext.HttpContext.Request.Headers;

            if (!headers.ContainsKey(header))
            {
                return false;
            }

            return string.Equals(headers[header], value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
