(function ($)
{
	if (typeof Directory == "undefined") { Directory = {}; }

	Directory.Directory = function (options)
	{
		Application.create(
		{
			options:
			{
			},

			init: function ()
			{
				var self = this;
				Application.prototype.init.apply(this);

				$('#pagerButtonBack').button({ text: false, icons: { primary: 'ui-icon-triangle-1-w' } });
				$('#pagerButtonNext').button({ text: false, icons: { primary: 'ui-icon-triangle-1-e'} });
				$('.pagerButtonNum').button();
			}

		}, options);
	};
} (jQuery));