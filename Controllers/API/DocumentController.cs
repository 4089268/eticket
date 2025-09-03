using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using eticket.Models;
using eticket.Data;

namespace eticket.Controllers.api;

[Authorize]
[Route("/api/{Controller}")]
public class DocumentController(ILogger<DocumentController> logger, TicketsMediaDBContext mediaContext) : ControllerBase
{
    private readonly ILogger<DocumentController> _logger = logger;
    private readonly TicketsMediaDBContext mediaContext = mediaContext;


    [HttpGet("{id}")]
    public IActionResult Document([FromRoute] string id)
    {
        Guid? documentId = Guid.TryParse(id, out Guid casted) ? casted : null;
        if (documentId == null)
        {
            return BadRequest(new
            {
                Title = "El identificador del documento es incorrecto"
            });
        }

        var oprImage = this.mediaContext.OprImagenes.FirstOrDefault(doc => doc.IdImagen == documentId);
        if (oprImage?.Documento == null)
        {
            return NotFound(new
            {
                Title = "El documento no existe o no esta disponible"
            });
        }

        var mediaType = oprImage.FileExtension?.ToLower() switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt" => "text/plain",
            _ => "application/octet-stream"
        };

        return File(oprImage.Documento!, mediaType);
    }
    
    [HttpGet("{id}/download")]
    public IActionResult Download([FromRoute] string id)
    {
        Guid? documentId = Guid.TryParse(id, out Guid casted) ? casted : null;
        if (documentId == null)
        {
            return BadRequest(new
            {
                Title = "El identificador del documento es incorrecto"
            });
        }

        var oprImage = this.mediaContext.OprImagenes.FirstOrDefault(doc => doc.IdImagen == documentId);
        if(oprImage?.Documento == null)
        {
            return NotFound( new {
                Title = "El documento no existe o no esta disponible"
            });
        }

        var mediaType = oprImage.FileExtension?.ToLower() switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt" => "text/plain",
            _ => "application/octet-stream"
        };

        return File(oprImage.Documento!, mediaType, oprImage.Descripcion ?? "eticket" + oprImage.FileExtension );
    }

}
