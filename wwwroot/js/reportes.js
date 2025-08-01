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

function uploadAttachFile(event)
{
    const url = '/reportes/upload-attach-file'
    const fileInput = event.target;
    if (fileInput.files.length === 0) return;

    const maxSize = 10 * 1024 * 1024; // 10MB
    if (fileInput.files[0].size > maxSize) {
        Swal.fire({
            title: "Archivo demasiado grande",
            text: "El archivo debe ser menor a 10MB.",
            icon: "error"
        });
        fileInput.value = "";
        return;
    }

    const label = document.getElementById('upload-label');
    const icon = label.querySelector('.upload-icon');
    const spinner = label.querySelector('.upload-spinner');

    // Toggle to spinner
    icon.classList.add('d-none');
    spinner.classList.remove('d-none');

    const formData = new FormData();
    formData.append('file', fileInput.files[0]);

    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function(response)
        {
            Swal.fire({
                title: "Archivo subido",
                text: "El archivo se ha subido correctamente.",
                icon: "success"
            });

            appendUploadedFile(response);
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
                Swal.fire({
                    title: "Error al subir el archivo",
                    text: error,
                    icon: "error"
                });
            }
        },
        complete: function() {
            setTimeout(() => {
                icon.classList.remove('d-none');
                spinner.classList.add('d-none');
            }, 500);
        }
    });
}

function appendUploadedFile(response)
{
    const { fileName, originalFileName} = response;
    var displayName = originalFileName.length > 120
        ? originalFileName.slice(0, 120) + "..."
        : originalFileName;

    // display a list element with the uploaded file
    var liElement = `<li class='list-box__item'><p><span>${displayName}</span></p></li>`;
    const ul = document.querySelector("ul#attachFiles");
    $("ul#attachFiles").append(liElement);


    // append a inputs with the file uploaded metadata
    const container = document.querySelector("#hiddenFileInputs");
    
    const index = ($("ul#attachFiles li").length) - 1;

    const guidNameInput = document.createElement("input");
        guidNameInput.type = "hidden";
        guidNameInput.name = `UploadedFiles[${index}][GuidName]`;
        guidNameInput.value = fileName;

    const originalNameInput = document.createElement("input");
        originalNameInput.type = "hidden";
        originalNameInput.name = `UploadedFiles[${index}][OriginalName]`;
        originalNameInput.value = originalFileName;

    container.appendChild(guidNameInput);
    container.appendChild(originalNameInput);
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