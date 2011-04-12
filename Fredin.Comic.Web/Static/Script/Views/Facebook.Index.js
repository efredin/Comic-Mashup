(function ($)
{
	if (typeof Facebook == "undefined") { Facebook = {}; }

	Facebook.Index = function (options)
	{
		Application.create(
		{
			options:
			{
				effects: []
			},

			init: function ()
			{
				var self = this;
				Application.prototype.init.apply(this);

				$('#buttonsetIntensity').buttonset();
				$('.ui-optionset').optionset();
				$('#effectSelector').select2(
				{
					multiple: false,
					data: self.options.effects,
					dataKey: 'EffectId',
					dataValue: 'Title',
					template: $('#templateEffect'),
					emptyText: 'No effects found.'
				});
				$('#effectSelector').select2('select', 0);

				$('#buttonRender')
					.button({ icons: { primary: 'ui-icon-image' } })
					.click(function ()
					{
						var button = $(this);
						if (self.options.user == null)
						{
							self.requestConnect(true, null, function ()
							{
								button.parents('form').submit();
							});
						}
						else
						{
							button.parents('form').submit();
						}
					});
			}

		}, options);
	};
} (jQuery));