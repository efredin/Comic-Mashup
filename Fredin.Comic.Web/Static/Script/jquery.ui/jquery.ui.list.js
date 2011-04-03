(function($)
{
	$.widget('ui.list',
	{
		options:
		{
			data: [],
			dataKey: null, // data item key member
			dataValue: null, // data item value member
			template: '<#= this #>',
			emptyText: ''
		},

		_create: function()
		{
			$(this.element)
				.addClass('ui-list')
				.append("<ul></ul><div class='ui-list-empty'>" + this.options.emptyText + "</div>");

    		this.dataBind();
		},

		dataBind: function()
		{
			var self = this;
    		var container = $('ul', this.element);

			var template = $.jqotec(this.options.template);

    		container.empty();
			if(this.options.data != null && this.options.data.length > 0)
			{
				$('.ui-list-empty', this.element).hide();
    			for (var i = 0; i < this.options.data.length; i++)
    			{
					var item = this.options.data[i];
					var key = this.options.dataKey != null ? item[this.options.dataKey] : item;
					var value = this.options.dataValue != null ? item[this.options.dataValue] : item;

					container.append("<li><a href='javascript:void(0)'></a></li>");
					$('li:last a', container).jqoteapp(template, item);
    			}

				var items = $('li', container);

//				// Event hooks
//				var itemIn = function(){ $(this).addClass('ui-state-hover'); };
//				var itemOut = function(){ $(this).removeClass('ui-state-hover'); };
//				var itemClick = function(event){ self.select(this, event); };

				items.addClass('ui-state-default ui-corner-all')
//					.hover(itemIn, itemOut)
//					.click(itemClick);
			}
			else
			{
				// Empty
				$('.ui-list-empty', this.element).show();
			}
		},

//		add: function(item)
//		{
//			this.insert(item, this.options.data.length);
//		},

//		insert: function(item, index)
//		{
//			this.options.data.splice(index, 0, item);
//			this.dataBind();
//		},

//		remove: function(index)
//		{
//			this.options.data.splice(index, 1);
//			this.dataBind();
//		},

		_setOption: function(key, value) 
		{
			$.Widget.prototype._setOption.apply(this, arguments);
			
			if(key == "data")
			{
				this.dataBind();
			}
		},

		filter: function(selector)
		{
			$('li', $(this.element).parent())
				.hide()
				.has(selector)
				.show();
		}
	});
} (jQuery));
