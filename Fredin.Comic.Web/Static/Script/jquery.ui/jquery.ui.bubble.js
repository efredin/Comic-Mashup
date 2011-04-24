(function ($)
{
	$.widget('ui.bubble',
	{
		options:
		{
			bubble: null, // ClientTextBubbleDirection
			allowDelete: false,
			onDelete: null
		},

		_create: function ()
		{
			var self = this;

			var change = function ()
			{
				var text = $(this).val()
					.replace(/<br\/>/ig, "\n")
					.replace(/</g, "&lt;")
					.replace(/>/g, "&gt;")
					.replace(/\r?\n/ig, "<br/>");

				// Sizer
				var sizer = $('.ui-bubbleSizer', $(self.element).parent()).html(text);
				$(this).css('width', sizer.width() + 10 + 'px')
					.css('height', sizer.height() + 'px');

				// Resize factor
				var factorX = $(self.element).innerWidth() / self.options.bubble.TextScaleX;
				var factorY = $(self.element).innerHeight() / self.options.bubble.TextScaleY;

				// Resize bubble image - don't allow bubbles to be so small they dissapear!
				var width = Math.max(self.options.bubble.BaseScaleX * factorX, 75);
				var height = Math.max(self.options.bubble.BaseScaleY * factorY, 50);
				$('.ui-bubbleImage', $(self.element).parent())
					.css('width', width + 'px')
					.css('height', height + 'px');

				// Position textarea
				var offsetX = (width - $(self.element).innerWidth()) / 2;
				var offsetY = (height - $(self.element).innerHeight()) / 2;
				$(this).css('margin', offsetY + 'px ' + offsetX + 'px');

				$('.ui-bubbleDelete', $(self.element).parent())
					.css('left', offsetX + sizer.width() + 10 + 'px')
					.css('top', (height / 2) - 4 + 'px');
			};

			var click = function ()
			{
				$(self.element).focus();
			};

			var kill = function ()
			{
				console.log('killing machine!');
				self._trigger('onDelete');
				$(self.element).parent().remove(); // remove from dom
				self.destroy(); // destroy widget
			};

			$(this.element).wrap("<div class='ui-bubble'></div>")
				.addClass('ui-bubbleText')
				.keydown(change)
				.keyup(change)
				.change(change);

			$(this.element).parent()
				.mouseup(click)
				.append("<img class='ui-bubbleImage' src='" + self.options.bubble.ImageUrl + "' />")
				.append("<div class='ui-bubbleSizer'></div>");

			if (this.options.allowDelete)
			{
				$(this.element).parent()
					.append("<a class='ui-bubbleDelete' href='javascript:void(0);'></a>");

				$('.ui-bubbleDelete', $(this.element).parent())
					.click(kill);
			}

			$(this.element).change();
		}
	});
} (jQuery));
