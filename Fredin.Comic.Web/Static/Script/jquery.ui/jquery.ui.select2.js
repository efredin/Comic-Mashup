(function($)
{
	$.widget('ui.select2',
	{
		options:
		{
			multiple: true,
			data: null,
			dataKey: null, // data item key member
			dataValue: null, // data item value member
			template: '<#= this #>',
			selected: null, // event handler fires after selection
			unselected: null, // event handler fires after unselection
			emptyText: ''
		},

		_create: function()
		{
			$(this.element)
				.wrap("<div class='ui-select'></div>")
				.hide()
				.parent()
				.append("<ul></ul><div class='ui-select-empty'>" + this.options.emptyText + "</div>");

    		this.dataBind();
		},

		dataBind: function()
		{
			var self = this;
			var select = $(this.element);
    		var container = $('ul', select.parent());

			var template = $.jqotec(this.options.template);

			if (this.options.data == null)
			{
				this._buildDataSet();
			}

    		container.empty();
			select.empty();
			select.append("<option value=''></option>");

			if(this.options.data != null && this.options.data.length > 0)
			{
				$('.ui-select-empty', select.parent()).hide();
				container.show();
    			for (var i = 0; i < this.options.data.length; i++)
    			{
					var item = this.options.data[i];
					var key = this.options.dataKey != null ? item[this.options.dataKey] : item;
					var value = this.options.dataValue != null ? item[this.options.dataValue] : item;

					select.append("<option value='"+key+"' >"+value+"</option>");
					container.append("<li><a href='javascript:void(0)'></a></li>");
					$('li:last a', container).jqoteapp(template, item);
    			}

				var items = $('li', select.parent());

				// Event hooks
				var itemIn = function(){ $(this).addClass('ui-state-hover'); };
				var itemOut = function(){ $(this).removeClass('ui-state-hover'); };
				var itemClick = function(event){ self.select(this, event); };

				items.addClass('ui-state-default ui-corner-all')
					.hover(itemIn, itemOut)
					.click(itemClick);
			}
			else
			{
				// Empty
				$('.ui-select-empty', select.parent()).show();
				container.hide();
			}
		},

		_buildDataSet: function ()
		{
			this.options.dataKey = 'key';
			this.options.dataValue = 'value';
			this.options.data = [];

			$(this.element).children('option').each(function (i, option)
			{
				this.options.data.push(
				{
					key: $(option).val(),
					value: $(option).html()
				});
			});
		},

		select: function(target, event)
		{
			var items = $('li', $(this.element).parent());
			var options = $('option', this.element);

			if(typeof target == "number")
			{
				target = items.eq(target);
			}
			else
			{
				target = $(target);
			}
			var index = items.index(target);
			
			if(!this.options.multiple)
			{
				var oldIndex = items.index($(':selected', options));

				items.removeClass('ui-state-active');
				target.addClass('ui-state-active');

				options.removeAttr('selected');
				options.eq(index + 1).attr('selected', 'selected');

				this._trigger('unselected', event, {item: this.options.data[oldIndex], index: oldIndex});
				this._trigger('selected', event, {item: this.options.data[index], index: index});
			}
			else
			{
				target.toggleClass('ui-state-active');

				if(target.hasClass('ui-state-active'))
				{
					options.eq(index + 1).attr('selected', 'selected');
					this._trigger('selected', event, {item: this.options.data[index], index: index});
				}
				else
				{
					options.eq(index + 1).removeAttr('selected');
					this._trigger('unselected', event, {item: this.options.data[index], index: index});
				}
			}

		},

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
			var size = $('li', $(this.element).parent())
				.hide()
				.has(selector)
				.show()
				.size();

			if(size == 0)
			{
				$('.ui-select-empty', $(this.element).parent()).show();
				$('ul', $(this.element).parent()).hide();
			}
			else
			{
				$('.ui-select-empty', $(this.element).parent()).hide();
				$('ul', $(this.element).parent()).show();
			}
		}
	});
} (jQuery));
