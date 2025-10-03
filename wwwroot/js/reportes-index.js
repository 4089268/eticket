function debounce(fn, delay) {
	let timer;
	return function (...args) {
		clearTimeout(timer);
		timer = setTimeout(() => {
			fn(...args);
		}, delay);
	}
}

function fetchData()
{
	const params = new URLSearchParams(filtroTablas).toString();
	window.location.search = params;
}

function initFilters()
{
	let selectTipoEntrada = document.getElementById("select-tipoentrada");
	selectTipoEntrada.value = filtroTablas.te;
	selectTipoEntrada.addEventListener("change", function(event)
	{
		filtroTablas.te = parseInt(event.target.value, 10);
		debouncedFetchData();
	});

	let selectTipoReporte = document.getElementById("select-tiporeporte");
	selectTipoReporte.value = filtroTablas.tr;
	selectTipoReporte.addEventListener("change", function(event)
	{
		filtroTablas.tr = parseInt(event.target.value, 10);
		debouncedFetchData();
	});

	let selectEstatus = document.getElementById("select-status");
	selectEstatus.value = filtroTablas.e;
	selectEstatus.addEventListener("change", function(event)
	{
		filtroTablas.e = parseInt(event.target.value, 10);
		debouncedFetchData();
	});

	let selectOficinas = document.getElementById("select-oficina");
	selectOficinas.value = filtroTablas.o;
	selectOficinas.addEventListener("change", function(event)
	{
		filtroTablas.o = parseInt(event.target.value, 10);
		debouncedFetchData();
	});
}

function mostrarMapa(lat, lon)
{
	console.dir(lat, lon);
	modalMapa.show();
	maps.addMarker(lat, lon);
}

var filtroTablas = {
	te: undefined, // Tipo Entrada
	tr: undefined, // Tipor Reporte
	e: undefined, // Estatus
	o: undefined //Oficina
};

const debouncedFetchData = debounce(fetchData, 750);

let modalMapa = undefined;

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
            mapId: "eticket_reportes_map_id"
        };
        maps.map = new google.maps.Map(document.getElementById("mapa"), mapProp);

        // add the marker
        if(showMarker)
        {
            const marker = new google.maps.marker.AdvancedMarkerElement({
                map: maps.map,
                position: myCenter,
                title: ''
            });
            marker.setMap(maps.map);
        }
    },
	addMarker: (lat, lang)=>{
		if (maps.map && maps.map.markers)
		{
			maps.map.markers.forEach(marker => marker.setMap(null));
			maps.map.markers = [];
		}
		else
			{
			maps.map.markers = [];
		}
		const marker = new google.maps.marker.AdvancedMarkerElement({
			map: maps.map,
			position: new google.maps.LatLng(lat, lang),
			title: ''
		});
		marker.setMap(maps.map);
		maps.map.markers.push(marker);

		maps.map.setCenter(new google.maps.LatLng(lat, lang));
		maps.map.setZoom(14);
	}
};

jQuery(document).ready(function()
{
	maps.init(null, null);
	modalMapa = new bootstrap.Modal(document.getElementById("mapa-modal"));

	$('table.reportes-table').DataTable({
		paging: true,
		ordering: false,
		scrollX: true,
		scrollY: 420,
		fixedColumns: {
			right: 1
		},
		buttons: [
			'copy', 'excel', 'pdf'
		],
		layout: {
			topStart: 'buttons'
		},
		language: {
			url: 'https://cdn.datatables.net/plug-ins/2.3.4/i18n/es-MX.json',
		},
	});

	filtroTablas.te = currentTipoEntrada;
	filtroTablas.tr = currentTipoReporte;
	filtroTablas.e = currentEstatus;
	filtroTablas.o = curtentOficina;

	// Init filter selections
	initFilters();
});