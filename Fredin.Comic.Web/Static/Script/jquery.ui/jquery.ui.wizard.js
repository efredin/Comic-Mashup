(function ($)
{
	$.widget('ui.wizard',
	{
		options:
		{
			step: 0, // active step index
			width: 400,
			validator: null,
			stepActive: null, // event fired when a new step becomes active
			stateField: 'step'
		},

		_create: function ()
		{
			var self = this;

			$(this.element).addClass('ui-wizard');

			var steps = $('.ui-wizard-step', this.element);
			var width = this.options.width ? this.options.width : $(this.element).innerWidth();

			if ($('.ui-wizard-steps', this.element).size() == 0)
			{
				steps.wrapAll("<div class='ui-wizard-steps' />");
			}
			if ($('.ui-wizard-scroll', this.element).size() == 0)
			{
				steps.wrapAll("<div class='ui-wizard-scroll' />");
			}
			$('.ui-wizard-steps', this.element).width(width);

			steps.width(width);
			$('.ui-wizard-scroll', this.element).width(width * steps.length);

			$('.ui-wizard-title a', this.element).addClass('ui-corner-all').disable();

			$('.ui-wizard-back', this.element).button({ icons: { primary: 'ui-icon-triangle-1-w'} })
				.click(function (event) { self.back(event); })

			$('.ui-wizard-next', this.element).button({ icons: { primary: 'ui-icon-triangle-1-e'} })
				.click(function (event) { self.next(event); })

			$(this.element).append("<div class='ui-wizard-clear'></div>");

			this.loadState();
			$(window).bind('hashchange', function () { self.loadState(); });
		},

		next: function (event)
		{
			this.step(this.options.step + 1, event);
		},

		back: function (event)
		{
			this.step(this.options.step - 1, event);
		},

		step: function (step, event)
		{
			var self = this;

			var steps = $('.ui-wizard-step', this.element);

			var newIndex = (typeof step == 'number') ? step : steps.index($(step));
			var max = steps.size();
			if (newIndex >= max)
			{
				newIndex = max - 1;
			}
			else if (newIndex < 0)
			{
				newIndex = 0;
			}

			// trigger validation when stepping forward
			if (newIndex > this.options.step && !this.validate())
			{
				return false;
			}

			var newStep = steps.eq(newIndex);
			$('.ui-wizard-steps', this.element).stop().scrollTo(newStep, 600);

			// Enable / Disable step
			steps.each(function (index)
			{
				if (index <= newIndex)
				{
					$(this).enable2();
				}
				else
				{
					$(this).disable();
				}
			});

			// titles
			$('.ui-wizard-title a', this.element).each(function (x)
			{
				$(this).enable2()
					.removeClass('ui-state-active')
					.unbind('click');

				if (x == newIndex)
				{
					$(this).addClass('ui-state-active');
				}
				else if (x > newIndex)
				{
					$(this).disable();
				}
				else
				{
					$(this).click(function () { self.step(x); })
						.attr('href', 'javascript:void(0)');
				}
			});


			// buttons
			var nextButton = $('.ui-wizard-next', this.element);
			if (newIndex < max - 1)
			{
				nextButton.show();
			}
			else
			{
				nextButton.hide();
			}

			var backButton = $('.ui-wizard-back', this.element);
			if (newIndex > 0)
			{
				backButton.show();
			}
			else
			{
				backButton.hide();
			}

			this.options.step = newIndex;
			var state = {};
			state[this.options.stateField] = newIndex;
			$.bbq.pushState(state);

			this._trigger('stepActive', event, { step: this.options.step });

			return true;
		},

		loadState: function ()
		{
			var step = $.bbq.getState(this.options.stateField, true) || 0;

			// If validation fails on init, start at 0
			if (!this.stepInit && step > 0 && !this.validate())
			{
				step = 0;
			}

			if (step != this.options.step || !this.stepInit)
			{
				this.stepInit = true;
				this.step(step);
			}
		},

		validate: function ()
		{
			var valid = true;
			if (this.options.validator != null)
			{
				// Use validator
				valid = this.options.validator.form();
			}
			return valid;
		}
	});
} (jQuery));
