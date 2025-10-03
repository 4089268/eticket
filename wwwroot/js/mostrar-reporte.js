let modalDocumento = undefined;

var modificacionReporte = {
    selectTipoReporte: undefined,
    selectEstatus: undefined,
    selectOficina: undefined,
    buttonSave:undefined,
    buttonDiscard: undefined,
    currentEstatus: undefined,
    currentTipoReporte: undefined,
    currentOficina: undefined,
    showSaveChangesButton: false,
    _newEstatus: undefined,
    _newTipoReporte: undefined,
    _newOficina: undefined,
    set newEstatus(value) {
        this._newEstatus = value;
        this.checkChanges();
    },
    get newEstatus(){
        return this._newEstatus;
    },
    set newTipoReporte(value) {
        this._newTipoReporte = value;
        this.checkChanges();
    },
    get newTipoReporte(){
        return this._newTipoReporte;
    },
    set newOficina(value) {
        this._newOficina = value;
        this.checkChanges();
    },
    get newOficina(){
        return this._newOficina;
    },
    load: ()=> {
        modificacionReporte.currentEstatus = _currentEstatus;
        modificacionReporte.currentTipoReporte = _currentTipoReporte;
        modificacionReporte.currentOficina = (_currentOficina) ? _currentOficina : 0;
        modificacionReporte._newEstatus = _currentEstatus;
        modificacionReporte._newTipoReporte = _currentTipoReporte;
        modificacionReporte._newOficina = (_currentOficina) ? _currentOficina : 0;

        // configure buttons
        modificacionReporte.buttonSave = document.getElementById('button-save-changes');
        modificacionReporte.buttonSave.addEventListener('click', function(event){
            modificacionReporte.saveChanges();
        });

        modificacionReporte.buttonDiscard = document.getElementById('button-discard-changes');
        modificacionReporte.buttonDiscard.addEventListener('click', function(event){
            modificacionReporte.discardChanges();
        })


        // configure select inputs
        modificacionReporte.selectTipoReporte = $('#select-tiporeporte')
            .select2()
            .val(modificacionReporte.newTipoReporte)
            .trigger('change')
            .on('select2:select', function (e) {
                var selectedValue = e.params.data.id;
                if (selectedValue !== modificacionReporte.newTipoReporte)
                {
                    modificacionReporte.newTipoReporte = selectedValue;
                }
            });

        modificacionReporte.selectEstatus = $('#select-estatus')
            .select2()
            .val(modificacionReporte.newEstatus)
            .trigger('change')
            .on('select2:select', function (e) {
                var selectedValue = e.params.data.id;
                if (selectedValue !== modificacionReporte.newEstatus)
                {
                    modificacionReporte.newEstatus = selectedValue;
                }
            });

        modificacionReporte.selectOficina = $('#select-oficinas')
            .select2()
            .val(modificacionReporte.newOficina)
            .trigger('change')
            .on('select2:select', function (e) {
                var selectedValue = e.params.data.id;
                if (selectedValue !== modificacionReporte.newOficina)
                {
                    modificacionReporte.newOficina = selectedValue;
                }
            });
    },
    checkChanges: ()=>{
        if( modificacionReporte.currentEstatus != modificacionReporte._newEstatus ||
            modificacionReporte.currentTipoReporte != modificacionReporte._newTipoReporte ||
            modificacionReporte.currentOficina != modificacionReporte._newOficina
        )
        {
            modificacionReporte.showSaveChangesButton = true;
            modificacionReporte.buttonDiscard.classList.remove("d-none");
            modificacionReporte.buttonSave.classList.remove("d-none");
        }
        else
        {
            modificacionReporte.showSaveChangesButton = false;
            modificacionReporte.buttonDiscard.classList.add("d-none");
            modificacionReporte.buttonSave.classList.add("d-none");
        }
    },
    discardChanges: ()=>{
        modificacionReporte._newEstatus = modificacionReporte.currentEstatus;
        modificacionReporte._newTipoReporte = modificacionReporte.currentTipoReporte;
        modificacionReporte._newOficina = modificacionReporte.currentOficina;

        modificacionReporte.selectEstatus.val(modificacionReporte._newEstatus).trigger('change');
        modificacionReporte.selectTipoReporte.val(modificacionReporte._newTipoReporte).trigger('change');
        modificacionReporte.selectOficina.val(modificacionReporte.currentOficina).trigger('change');

        modificacionReporte.checkChanges();
    },
    saveChanges: ()=>{
        const data = {
            EstatusId: modificacionReporte.newEstatus,
            TipoReporte: modificacionReporte.newTipoReporte,
            OficinaId: modificacionReporte.newOficina
        };

        $.ajax({
            url: `/api/reportes/${folioReporte}`,
            type: 'PATCH',
            data: data,
            success: function(response) {
                Swal.fire({
                    title: "Reporte actualizado",
                    icon: "success",
                    confirmButtonText: 'Aceptar'
                }).then(() => {
                    window.location.reload();
                });
            },
            error: function(xhr) {
                let titleMessage = "Error al actualizar el reporte";
                if (xhr.responseJSON && xhr.responseJSON.title) {
                    titleMessage = xhr.responseJSON.title;
                }
                Swal.fire({
                    title: titleMessage,
                    icon: "error"
                });
            }
        });
    }
};

var maps = {
    map: undefined,
    default_lat: 19.922385,
    default_lon: -96.826112,
    init: (lat, lang)=>{
        let showMarker = false;
        if(lat && lang )
        {
            showMarker = true;
            var myCenter = new google.maps.LatLng(lat, lang);
            var initZoom = 17;
        }
        else
        {
            var myCenter = new google.maps.LatLng(maps.default_lat, maps.default_lon);
            var initZoom = 7;
        }

        var myCenter = (lat && lang)
            ? new google.maps.LatLng(lat, lang)
            : new google.maps.LatLng(maps.default_lat, maps.default_lon);

        // init the map
        var mapProp = {
            center: myCenter,
            zoom: initZoom,
            scrollwheel: false,
            mapId: "eticket_reporte_map_id"
        };
        maps.map = new google.maps.Map(document.getElementById("google-map"), mapProp);

        // add the marker
        if(showMarker)
        {
            const marker = new google.maps.marker.AdvancedMarkerElement({
                map: maps.map,
                position: myCenter,
                title: 'Uluru'
            });
            marker.setMap(maps.map);
        }
    }
};

function inicializarTabla()
{
    $('.adv-table').footable({
        "sorting": {
            "enabled": false
        },
        "paging": {
            "enabled": true,
            "current": 1
        },
        strings: {
            enabled: false
        },
        "filtering": {
            "enabled": false
        }
    });
}

function cargarFormularioEntrada()
{
    $("#nueva-entrada-wrap").load(_urlFormularioDetEntrada, null, onFormularioLoadedCompleted);
}

function onFormularioLoadedCompleted(resp, status, request)
{
    $('#observaciones').trumbowyg({
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
}

function submitReporteForm(event)
{
    event.preventDefault();

    const url = `/reportes/${folioReporte}/entrada`;
    const method = 'POST';
    const form = $(event.target);

    // clear errors messags
    $("span.text-danger").text("");

    $.ajax({
        url: url,
        type: method,
        data: form.serialize(),
        success: function(response) {
            Swal.fire({
                title: "Entrada registrada",
                icon: "success",
                confirmButtonText: 'Aceptar'
            }).then((result) => {
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

function mostrarDocumento(event, nombre, urlArchivo)
{
    $("body").css("cursor", "wait");

    const $iframe = $("#body-modal2 iframe");
    $iframe.off("load");
    $iframe.on("load", function ()
    {
        $("body").css("cursor", "default");
        $("#modaltitle2").text(nombre);
        modalDocumento.show();
    });
    $iframe.attr("src", urlArchivo);
}

function eliminarReporteButonClick(event)
{
    Swal.fire({
        title: "Confirmar eliminación",
        text: `Para eliminar escribe el folio del reporte: "${folioReporte}". \nEsta accion no se podrás revertir.`,
        icon: "warning",
        input: "text",
        inputPlaceholder: "Escribe el folio del reporte",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Eliminar",
        cancelButtonText: "Cancelar",
        inputValidator: (value) => {
            if (!value)
            {
                return "Debes escribir algo";
            }
            if (value !== folioReporte)
            {
                return "El folio del reporte no coincide";
            }
            return null;
        }
    }).then((result) => {
        if (result.isConfirmed)
        {
            eliminarReporte();
        }
    });
}

function eliminarReporte()
{
    const url = `/reportes/${folioReporte}/eliminar`;
    const method = 'DELETE';

    $.ajax({
        url: url,
        type: method,
        success: function(response)
        {
            Swal.fire({
                title: response.title,
                icon: "success",
                confirmButtonText: 'Aceptar'
            }).then((result) => {
                window.location.href = "/reportes";
            });
        },
        error: function(xhr, status, error) {
            console.error(xhr, error);
            let titleMessage = "Error no controlado al eliminar el reporte";
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
    });
}

jQuery(document).ready(function()
{
    if (document.getElementsByClassName("documento-adjunto-modal").length)
    {
        modalDocumento = new bootstrap.Modal(document.getElementById("documento-adjunto-modal"));
    }
    
    document.getElementById("button-delete").addEventListener("click", eliminarReporteButonClick);

    cargarFormularioEntrada();
    
    inicializarTabla();

    modificacionReporte.load();

    if($('#google-map').length)
    {
        maps.init(_lat, _lon);
    }
});