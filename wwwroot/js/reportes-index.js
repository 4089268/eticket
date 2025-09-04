FooTable.ReportesFiltering = FooTable.Filtering.extend({
	construct: function(instance){
		this._super(instance);
		
		this.statuses = ['Abierto', 'En Proceso', 'Atendido', 'Cancelado'];
		this.statusDefault = 'Todos';
		this.$status = null;

		this.reportTypes = ['Fuga', 'Falta de Servicio', 'Bacheo/Hundimiento', 'Falta de Tapa o Rejillas', 'Aclaracion de recibo', 'Quejas', 'Sugerencias', 'Otros'];
		this.reportTypeDefault = 'Todos';
		this.$reportType = null;

		this.registerTypes = ['Call Center', 'Chat Bot'];
		this.registerTypeDefault = 'Todos';
		this.$registerType = null;
	},
	$create: function(){
		this._super();
		var self = this;

		// create the status form group and dropdown
		var $status_form_grp = $('<div/>', {'class': 'form-group dm-select d-flex align-items-center adv-table-searchs__position my-xl-2 my-1 me-2'})
			.append($('<label/>', {'class': 'd-flex align-items-center mb-sm-0 mb-2', text: 'Estatus'}))
			.prependTo(self.$form);

		self.$status = $('<select/>', { 'class': 'form-control ms-sm-10 ms-0' })
			.on('change', {self: self}, self._onStatusDropdownChanged)
			.append($('<option/>', {text: self.statusDefault}))
			.appendTo($status_form_grp);

		$.each(self.statuses, (i, status) => self.$status.append($('<option/>').text(status)) );


		// create the report-type form group and dropdown
		var $reportType_form_grp = $('<div/>', {'class': 'form-group dm-select d-flex align-items-center adv-table-searchs__position my-xl-2 my-1 me-2'})
			.append($('<label/>', {'class': 'd-flex align-items-center mb-sm-0 mb-2', text: 'Tipo de Reporte'}))
			.prependTo(self.$form);

		self.$reportType = $('<select/>', { 'class': 'form-control ms-sm-10 ms-0' })
			.on('change', {self: self}, self._onReportTypeDropdownChanged)
			.append($('<option/>', {text: self.reportTypeDefault}))
			.appendTo($reportType_form_grp);

		$.each(self.reportTypes, (i, rtype) => self.$reportType.append($('<option/>').text(rtype)) );

		// create the register-type form group and dropdown
		var $registerType_form_grp = $('<div/>', {'class': 'form-group dm-select d-flex align-items-center adv-table-searchs__position my-xl-2 my-1 me-2'})
			.append($('<label/>', {'class': 'd-flex align-items-center mb-sm-0 mb-2', text: 'Tipo de Entrada'}))
			.prependTo(self.$form);

		self.$registerType = $('<select/>', { 'class': 'form-control ms-sm-10 ms-0' })
			.on('change', {self: self}, self._onRegisterTypeDropdownChanged)
			.append($('<option/>', {text: self.registerTypeDefault}))
			.appendTo($registerType_form_grp);

		$.each(self.registerTypes, (i, rtype) => self.$registerType.append($('<option/>').text(rtype)) );

	},
	_onStatusDropdownChanged: function(e){
		var self = e.data.self,
			selected = $(this).val();

		if (selected !== self.statusDefault) {
			self.addFilter('status', selected, ['status']);
		} else {
			self.removeFilter('status');
		}
		self.filter();
	},
	_onReportTypeDropdownChanged: function(e){
		var self = e.data.self,
			selected = $(this).val();

		if (selected !== self.reportTypeDefault) {
			self.addFilter('reportType', selected, ['reportType']);
		} else {
			self.removeFilter('reportType');
		}
		self.filter();
	},
	_onRegisterTypeDropdownChanged: function(e){
		var self = e.data.self,
			selected = $(this).val();

		if (selected !== self.registerTypeDefault) {
			self.addFilter('registerType', selected, ['registerType']);
		} else {
			self.removeFilter('registerType');
		}
		self.filter();
	},
	draw: function(){
		this._super();
		
		// handle the status filter if one is supplied
		var status = this.find('status');
		if (status instanceof FooTable.Filter) {
			this.$status.val(status.query.val());
		} else {
			this.$status.val(this.statusDefault);
		}

		// handle the reportType filter if one is supplied
		var reportType = this.find('reportType');
		if (reportType instanceof FooTable.Filter) {
			this.$repotType.val(reportType.query.val());
		} else {
			this.$reportType.val(this.reportTypeDefault);
		}

		// handle the registerType filter if one is supplied
		var registerType = this.find('registerType');
		if (registerType instanceof FooTable.Filter) {
			this.$registerType.val(registerType.query.val());
		} else {
			this.$registerType.val(this.registerTypeDefault);
		}
	}
});

$(function() {
	$('table.reportes-table').footable({
		"components": {
			"filtering": FooTable.ReportesFiltering
		},
	});
});