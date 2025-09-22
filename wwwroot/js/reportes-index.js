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
}


var filtroTablas = {
	te: undefined, // Tipo Entrada
	tr: undefined, // Tipor Reporte
	e: undefined // Estatus
};

const debouncedFetchData = debounce(fetchData, 750);

jQuery(document).ready(function()
{
	$('table.reportes-table').DataTable({
		paging: true,
		ordering: true,
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

	// Init filter selections
	initFilters();
});