(function ($)
{
	if (typeof User == "undefined") { User = {}; }

	User.Settings = function (options)
	{
		Application.create
		({
			options:
			{
			},

			init: function ()
			{
				var self = this;
				Application.prototype.init.apply(this);

				$('.ui-optionset').optionset();

				var subscribeChange = function()
				{
					if($('#Engage_Subscribe').is(':checked'))
					{
						$('#engage-setting').show(200);
					}
					else
					{
						$('#engage-setting').hide(200);
					}
				};

				$('#Engage_Subscribe').change(subscribeChange);
				subscribeChange();

				/* Validation */
				var validator = $('form', '#settings').validate
				({
					debug: false,
					errorClass: 'ui-state-highlight',
					rules:
					{
						Email: { required: true, email: true },
					},
					messages:
					{
						Email: { required: 'Please enter an email address', email: 'Invalid email address' }
					}
				});
			}

		}, options);
	};
} (jQuery));