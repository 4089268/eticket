function displayMenu()
{
    $("#form-section").load("/reportes/partial-view/menu");
}

function displayForm(reporteTipeId)
{
    currentReportTypeId = reporteTipeId;
    const url = `/reportes/partial-view/formulario?tipoReporteId=${reporteTipeId}`;
    $("#body-modal").load(url, null, onFormularioLoadedCompleted);
}

function displayTable()
{
    const url = '/reportes/partial-view/ultimos-reportes';
    $('#table-section').load(url);
}

function onFormularioLoadedCompleted(resp, status, request)
{
    $('#Observaciones').trumbowyg({
        svgPath: '../img/svg/icons.svg',
        btns: [
            ['formatting'],
            ['strong', 'em', 'del'],
            ['superscript', 'subscript'],
            ['link'],
            ['base64'],
            ['justifyLeft', 'justifyCenter', 'justifyRight', 'justifyFull'],
            ['unorderedList', 'orderedList'],
            ['emoji']
        ]
    });

    $("#nuevoReporteForm").on('submit', submitNuevoReporte);

    modal.show();
}

function submitNuevoReporte(event)
{
    event.preventDefault();

    const url = "/reportes";
    const method = 'POST';
    const form = $(event.target);

    // clear errors messags
    $("span.text-danger").text("");

    $.ajax({
        url: url,
        type: method,
        data: form.serialize(),
        success: function(response) {
            modal.hide();
            
            var {folio} = response;
            Swal.fire({
                title: "Reporte almacenado",
                text: `Folio: ${folio}`,
                icon: "success",
                confirmButtonText: 'Aceptar'
            }).then((result) => {
                // TODO: Reload the table
                window.location.reload();
            });
        },
        error: function(xhr, status, error) {
            console.error(xhr, error);
            if(xhr.status == 422)
            {
                const {errors} = xhr.responseJSON;
                errors.forEach(err => {
                    const errorSpan = $(`span[data-valmsg-for="${err.field}"]`);
                    errorSpan.text(err.message);
                });
            }
            else
            {
                let titleMessage = "Error no controlado al registrar la entrada";
                if(xhr.responseJSON)
                {
                    const { title } = xhr.responseJSON;
                    if(title)
                    {
                        titleMessage = title;
                    }

                }
                Swal.fire({
                    title: titleMessage,
                    icon: "error"
                });
            }
        }
    });
}

jQuery(document).ready(function()
{
    displayMenu();
    displayTable();

    // * init the modal
    if (document.getElementsByClassName("nuevo-reporte-modal").length)
    {
        modal = new bootstrap.Modal(document.getElementById("nuevo-reporte-modal"));
    }
});