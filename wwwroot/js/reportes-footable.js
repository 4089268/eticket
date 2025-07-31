FooTable.MyFiltering = FooTable.Filtering.extend({
	construct: function(instance){
		this._super(instance);
		// props for the first dropdown
		this.statuses = ['Abierto', 'En Proceso', 'Atendido', 'Cancelado'];
		this.statusDefault = 'Todos';
		this.$status = null;
	},
	$create: function(){
		this._super();
		var self = this;
		// create the status form group and dropdown
		var $status_form_grp = $('<div/>', {'class': 'form-group dm-select d-flex align-items-center adv-table-searchs__position my-xl-25 my-15 me-sm-20 me-0 '})
			.append($('<label/>', {'class': 'd-flex align-items-center mb-sm-0 mb-2', text: 'position'}))
			.prependTo(self.$form);

		self.$status = $('<select/>', { 'class': 'form-control ms-sm-10 ms-0' })
			.on('change', {self: self}, self._onStatusDropdownChanged)
			.append($('<option/>', {text: self.statusDefault}))
			.appendTo($status_form_grp);

		$.each(self.statuses, function(i, status){
			self.$status.append($('<option/>').text(status));
		});

	},
	_onStatusDropdownChanged: function(e){
		var self = e.data.self,
			selected = $(this).val();
		if (selected !== self.statusDefault){
			self.addFilter('position', selected, ['position']);
		} else {
			self.removeFilter('position');
		}
		self.filter();
	},
	draw: function(){
		this._super();
		// handle the status filter if one is supplied
		var status = this.find('position');
		if (status instanceof FooTable.Filter){
			this.$status.val(status.query.val());
		} else {
			this.$status.val(this.statusDefault);
		}
	}
});

$(function() {
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
		},
		components: {
			filtering: FooTable.MyFiltering
		},
	});
});