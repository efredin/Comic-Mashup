(function ($)
{
	if (typeof Help == "undefined") { Help = {}; }

	Help.Contact = function (options)
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

				$('#contactButton').button({ icons: { primary: 'ui-icon-mail-closed'} });

				/* Validation */
				var validator = $('form', '#contact').validate
				({
					debug: false,
					errorClass: 'ui-state-highlight',
					rules:
					{
						Nickname: { required: true },
						Email: { required: true, email: true },
						Message: { required: true, minlength: 10 }
					},
					messages:
					{
						Nickname: { required: 'Please enter a name' },
						Email: { required: 'Please enter an email address', email: 'Invalid email address' },
						Message: { required: 'Please enter a message', minlength: 'Your message is too short' }
					}
				});
			}

		}, options);
	};
} (jQuery));