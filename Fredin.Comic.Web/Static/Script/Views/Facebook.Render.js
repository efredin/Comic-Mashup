(function ($)
{
	if (typeof Facebook == "undefined") { Facebook = {}; }

	Facebook.Render = function (options)
	{
		Application.create(
		{
			options:
			{
				task: null
			},

			init: function ()
			{
				var self = this;
				Application.prototype.init.apply(this);

				$('#renderLoad').load({ content: '#renderContent' });
				$('#renderShare')
					.button({ icons: { primary: 'ui-icon-star'} })
					.click(function () { self.shareFb(); });

				$('#renderBack').button({ icons: { primary: 'ui-icon-triangle-1-w' } });

				var timeout = 1000 * 30; // 30 second timeout!
				var progressFrequency = 1000 * 5; // check every 5 seconds
				var progressTime = 0;
				var progressInterval = null;
				var progressCallback = function ()
				{
					progressTime += progressFrequency;
					if (progressTime > timeout)
					{
						window.clearInterval(progressInterval);
						self.error("Unable to render your photo. Please try again.");
					}

					// get progress from server
					$.ajax
					({
						dataType: 'json',
						type: 'GET',
						url: self.options.baseHref + 'Facebook/RenderProgress',
						data: { taskId: self.options.task.TaskId },
						success: function (data, textStatus, request)
						{
							if (data)
							{
								self.options.task = data;
								if (self.options.task.Status == 2)
								{
									// Render complete
									window.clearInterval(progressInterval);
									$('#renderPhoto').html('<img src="' + self.options.task.RenderUrl + '" alt="" />');
									$('#renderLoad').load("complete");
								}
								else if (self.options.task.Status == 3)
								{
									window.clearInterval(progressInterval);
									self.error('Unable to render your photo. Please try again');
									$('#renderLoad').load("complete");
								}
							}
						},
						error: function (xhr, textStatus, x)
						{
							// Prevent application default error handler from firing
						}
					});
				}
				progressInterval = window.setInterval(progressCallback, progressFrequency);
			},

			shareFb: function ()
			{
				FB.ui
				({
					method: 'apprequests',
					message: 'Check out this cool app that transforms your profile picture into something cool.',
					title: 'Comic Mashup Profile Photo'
				});
			}

		}, options);
	};
} (jQuery));