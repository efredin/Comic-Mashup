(function($)
{
	$.widget('ui.optionset',
	{
		options:
		{
			title: null,
			caption: null,
			content: null
		},

		_create: function()
		{
			var self = this;

			$(this.element).addClass('ui-widget ui-widget-content ui-corner-all ui-optionset');

			if(this.options.title == null)
			{
				this.options.title = $('.ui-optionset-title', this.element);
			}
			else
			{
				$(this.options.title).addClass('ui-optionset-title');
			}

			if(this.options.caption == null)
			{
				this.options.caption = $('.ui-optionset-caption', this.element);
			}
			else
			{
				$(this.options.caption).addClass('ui-optionset-caption');
			}

			if(this.options.content == null)
			{
				this.options.content = $('.ui-optionset-content', this.element);
			}
			else
			{
				$(this.options.content).addClass('ui-optionset-content');
			}
		}
	});
} (jQuery));
