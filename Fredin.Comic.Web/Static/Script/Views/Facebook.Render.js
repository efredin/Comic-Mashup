(function ($)
{
	if (typeof Facebook == "undefined") { Facebook = {}; }

	Facebook.Render = function (options)
	{
		Application.create(
		{
			options:
			{
				task: null,
				autoShareFeed: false
			},

			init: function ()
			{
				var self = this;
				Application.prototype.init.apply(this);

				$('#renderLoad').load({ content: '#renderContent' });
				$('#renderShareRequest')
					.button({ icons: { primary: 'ui-icon-comment'} })
					.click(function () { self.shareRequest(); });
				$('#renderShareFeed')
					.button({ icons: { primary: 'ui-icon-star'} })
					.click(function () { self.shareFeed(); });

				$('#renderBack').button({ icons: { primary: 'ui-icon-triangle-1-w'} });

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
									if (self.options.autoShareFeed === true)
									{
										self.options.autoShareFeed = false;
										self.shareFeed();
									}
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

			shareFeed: function ()
			{
				FB.ui
				({
					method: 'feed',
					from: this.options.user.uid,
					description: 'I just used Comic Mashup for Profiles to transform my profile picture into a comic!',
					name: 'Comic Mashup Profile Photo',
					caption: 'http://apps.facebook.com/comicmashup/',
					picture: this.options.task.RenderUrl,
					link: 'http://apps.facebook.com/comicmashup/Facebook/'
				});
			},

			shareRequest: function ()
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