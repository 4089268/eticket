using System;
using eticket.Data;
using eticket.Models;
using eticket.ViewModels;
using Microsoft.Extensions.Options;

namespace eticket.Services;

public class DocumentosService(ILogger<ReportService> logger, TicketsMediaDBContext context, IOptions<TempPathSettings> tempPathOptions)
{
    private readonly ILogger<ReportService> logger = logger;
    private readonly TicketsMediaDBContext context = context;
    private readonly TempPathSettings tempPathSettings = tempPathOptions.Value;

    /// <summary>
    /// Stores a document associated with a given report folio in the database.
    /// </summary>
    /// <param name="fileMetadata"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException">Archivo no encontrado</exception>
    public async Task<Guid> AlmacenarDocumento(UploadedFileMetadata fileMetadata, long folioReporte, long folioDetalleReporte, int idUsuario)
    {
        var folderPath = tempPathSettings.Path + "attach-files/";
        var filePath = folderPath + fileMetadata.GuidName;

        if (!System.IO.File.Exists(filePath))
        {
            this.logger.LogWarning("No se encontro el archivo: {file}", filePath);
            throw new FileNotFoundException("Archivo no encontrado: {FilePath}", filePath);
        }

        // * almacenar archivo
        OprImagene record;
        try
        {
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            record = new OprImagene
            {
                FolioReporte = folioReporte,
                FolioReporteDetalle = folioDetalleReporte,
                IdInsert = idUsuario.ToString(),
                FechaInsert = DateTime.Now,
                Documento = fileBytes,
                FileExtension = Path.GetExtension(filePath),
                Filesize = fileBytes.Length,
                Descripcion = fileMetadata.OriginalName
            };
            this.context.OprImagenes.Add(record);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al procesar el archivo '{FileName}'. Ruta: {FilePath}", fileMetadata.GuidName, filePath);
            throw;
        }

        try
        {
            await context.SaveChangesAsync();
            logger.LogInformation("Archivo '{id}' guardado correctamente para el folio {FolioReporte}", record.IdImagen, folioReporte);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al guardar cambios en la base de datos para el folio {FolioReporte}", folioReporte);
            throw;
        }
        return record.IdImagen;
    }

}
