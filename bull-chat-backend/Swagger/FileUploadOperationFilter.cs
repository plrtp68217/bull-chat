using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace bull_chat_backend.Swagger
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasFileUpload = context.MethodInfo.GetParameters().Any(
                p => p.ParameterType == typeof(Microsoft.AspNetCore.Http.IFormFile) ||
                p.ParameterType == typeof(Microsoft.AspNetCore.Http.IFormFileCollection) ||
                (p.ParameterType.IsGenericType && p.ParameterType.GetGenericArguments().Contains(typeof(Microsoft.AspNetCore.Http.IFormFile))));

            if (!hasFileUpload)
                return;

            operation.RequestBody = new OpenApiRequestBody
            {
                Content =
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties =
                                {
                                    ["file"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        Format = "binary"
                                    }
                                },
                                Required = { "file" }
                            }
                        }
                    }
            };

            // Убираем параметры из operation.Parameters, чтобы не дублировались
            operation.Parameters.Clear();
        }
    }
}
