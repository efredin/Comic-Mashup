(function ($)
{
	$.fn.extend(
	{
		disable: function(children)
		{
			if(children !== false)
			{
				$(':input', this).attr('disabled', true)
					.addClass('ui-state-disabled');
			}

			return $(this).attr('disabled', true)
				.addClass('ui-state-disabled');
		},
		// Seems enable is conflicting
		enable2: function(children)
		{
			if(children !== false)
			{
				$(':input', this).removeAttr('disabled')
					.removeClass('ui-state-disabled');
			}

			return $(this).removeAttr('disabled')
				.removeClass('ui-state-disabled');
		}
	});
})(jQuery);
