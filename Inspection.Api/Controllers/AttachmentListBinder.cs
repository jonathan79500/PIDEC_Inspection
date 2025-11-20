using Inspection.Application.DTOs.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

public class AttachmentListBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var form = bindingContext.HttpContext.Request.Form;
        var files = form.Files;
        var result = new List<AttachmentDto>();

        int index = 0;
        while (true)
        {
            var idKey = $"Attachments[{index}].Id";
            if (!form.ContainsKey(idKey) && files.All(f => f.Name != $"Attachments[{index}].Content"))
                break; // no more attachments

            var attachment = new AttachmentDto
            {
                Id = form[idKey],
                Name = form[$"Attachments[{index}].Name"],
                Version = form[$"Attachments[{index}].Version"],
                Description = form[$"Attachments[{index}].Description"],
                Content = files.FirstOrDefault(f => f.Name == $"Attachments[{index}].Content")
            };

            result.Add(attachment);
            index++;
        }

        bindingContext.Result = ModelBindingResult.Success(result);
        return Task.CompletedTask;
    }
}
