let modal = undefined;
let modalDocumento = undefined;

var modificacionReporte = {
    selectTipoReporte: undefined,
    selectEstatus: undefined,
    buttonSave:undefined,
    buttonDiscard: undefined,
    currentEstatus: undefined,
    currentTipoReporte: undefined,
    showSaveChangesButton: false,
    _newEstatus: undefined,
    _newTipoReporte: undefined,
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
    load: ()=> {
        modificacionReporte.currentEstatus = _currentEstatus;
        modificacionReporte.currentTipoReporte = _currentTipoReporte;
        modificacionReporte._newEstatus = _currentEstatus;
        modificacionReporte._newTipoReporte = _currentTipoReporte;

        modificacionReporte.buttonSave = document.getElementById('button-save-changes');
        modificacionReporte.buttonSave.addEventListener('click', function(event){
            modificacionReporte.saveChanges();
        });

        modificacionReporte.buttonDiscard = document.getElementById('button-discard-changes');
        modificacionReporte.buttonDiscard.addEventListener('click', function(event){
            modificacionReporte.discardChanges();
        })

        
        modificacionReporte.selectTipoReporte = document.getElementById('select-tiporeporte');
        modificacionReporte.selectTipoReporte.value = modificacionReporte.newTipoReporte;
        modificacionReporte.selectTipoReporte.addEventListener('change', function(event) {
            const selectedValue = event.target.value;
            if (selectedValue !== modificacionReporte.newTipoReporte)
            {
                modificacionReporte.newTipoReporte = selectedValue;
            }
        });

        modificacionReporte.selectEstatus = document.getElementById('select-estatus');
        modificacionReporte.selectEstatus.value = modificacionReporte.newEstatus;
        modificacionReporte.selectEstatus.addEventListener('change', function(event) {
            const selectedValue = event.target.value;
            if (selectedValue !== modificacionReporte.newEstatus)
            {
                modificacionReporte.newEstatus = selectedValue;
            }
        });
    },
    checkChanges: ()=>{
        if(modificacionReporte.currentEstatus != modificacionReporte._newEstatus || modificacionReporte.currentTipoReporte != modificacionReporte._newTipoReporte)
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
        modificacionReporte.selectEstatus.value = modificacionReporte._newEstatus;
        modificacionReporte.selectTipoReporte.value = modificacionReporte._newTipoReporte;
        modificacionReporte.checkChanges();
    },
    saveChanges: ()=>{
        const data = {
            EstatusId: modificacionReporte.newEstatus,
            TipoReporte: modificacionReporte.newTipoReporte
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

function cargarFormulario()
{
    $("#body-modal").load(_urlFormularioDetEntrada, null, onFormularioCompleted);
}

function onFormularioCompleted(resp, status, request)
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
            modal.hide();
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

jQuery(document).ready(function()
{
    if (document.getElementsByClassName("nueva-entrada-modal").length)
    {
        modal = new bootstrap.Modal(document.getElementById("nueva-entrada-modal"));
    }

    if (document.getElementsByClassName("documento-adjunto-modal").length)
    {
        modalDocumento = new bootstrap.Modal(document.getElementById("documento-adjunto-modal"));
    }

    cargarFormulario();
    
    inicializarTabla();

    modificacionReporte.load();

    if($('#google-map').length)
    {
        maps.init(_lat, _lon);
    }
});