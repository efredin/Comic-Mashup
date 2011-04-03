(function($)
{
	$.widget("ui.input",
	{
		_create: function()
		{
			var self = this;

			var cssOn, cssOff;
			if($(this.element).is(':checkbox'))
			{
				cssOn = 'ui-icon-check';
				cssOff = 'ui-icon-blank';
			}
			else if($(this.element).is(':radio'))
			{
				cssOn = 'ui-icon-bullet';
				cssOff = 'ui-icon-radio-on';
			}
			
			// initial state
			var cssInit = $(this.element).is(':checked') ? cssOn : cssOff;

			var mouseIn = function(){ $(this).addClass('ui-state-hover'); };
			var mouseOut = function(){ $(this).removeClass('ui-state-hover'); };
			var change = function()
			{
				if($(this).is(':radio') && $(this).is(':checked'))
				{
					$('input[name=' + $(this).attr('name') + ']').not(':checked').change();
				}

				var icon = $(this).siblings('.ui-icon');
				if($(this).is(':checked'))
				{
					icon.removeClass(cssOff).addClass(cssOn);
				}
				else
				{
					icon.removeClass(cssOn).addClass(cssOff);
				}
				$(this).parent().toggleClass('ui-state-active', $(this).is(':checked')); 
			}
			var toggle = function() 
			{
				var input = $(this).siblings('input');

				if(input.is(':checkbox'))
				{
					input.attr('checked', input.attr('checked') !== true);
				}
				else
				{
					input.attr('checked', true);
				}

				// Force change event
				input.change();
			};

			// TODO: Listen for keyboard toggle args

			$(this.element)
				.hide()
				.change(change)
				.wrap('<div/>');

			$(this.element).parent()
				.addClass('ui-input ui-state-default ui-corner-all')
				.append('<a href="javascript:void(0)" class="ui-icon"/>')
				.hover(mouseIn, mouseOut);

			$('.ui-icon', $(this.element).parent())
				.addClass(cssInit)
				.click(toggle);
		}
	});
} (jQuery));
