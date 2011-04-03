(function ($)
{
	$.widget('ui.load',
	{
		options:
		{
			text: 'Loading. Please wait.',
			content: null,
			callback: null,
			progressbar: false
		},

		_create: function ()
		{
			$(this.element).addClass('ui-load');

			// Inner HTML / Text
			var text = $(this.element).html();
			if (text != '')
			{
				this.options.text = text;
			}

			$(this.element).empty();
			if (!this.options.progressbar)
			{
				$(this.element).append("<div class='ui-loadimg'></div>");
				$(this.element).append("<div class='ui-loadtext'>" + this.options.text + "</div>");
			}
			else
			{
				$(this.element).append("<div class='ui-progressbar'><div class='ui-loadtext ui-state-disabled'>" + this.options.text + "</div></div>");
				$('.ui-progressbar', this.element).progressbar({ value: 0 });
			}

			this.reset();
		},

		reset: function ()
		{
			$(this.element).show();
			$(this.options.content).css('visibility', 'hidden');

			if (this.options.progressbar)
			{
				$('.ui-progressbar', this.element).progressbar("option", "value", 0);
			}
		},

		complete: function ()
		{
			$(this.element).hide();
			$(this.options.content).css('visibility', 'visible');
			if ($.isFunction(this.options.callback))
			{
				this.options.callback();
			}
		},

		progress: function (percent)
		{
			if (this.options.progressbar)
			{
				$('.ui-progressbar', this.element).progressbar('value', percent);
			}
		}
	});
} (jQuery));
