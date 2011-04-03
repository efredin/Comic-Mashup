(function ($)
{
	if (typeof Comic == "undefined") { Comic = {}; }

	Comic.Read = function (options)
	{
		Application.create(
		{
			options:
			{
				comic: null
			},

			init: function ()
			{
				var self = this;
				Application.prototype.init.apply(this);

				// Reader rating buttons
				$('#readerFunny').button().click(function () { self.rate(this, 'ReaderFunny'); });
				$('#readerSmart').button().click(function () { self.rate(this, 'ReaderSmart'); });
				$('#readerRandom').button().click(function () { self.rate(this, 'ReaderRandom'); });

				// Reader action buttons
				$('#readerFb').button().click(function () { self.shareFb(); });

				// Tooltips
				$('#reader label').tooltip(
				{
					position:
					{
						my: "center bottom",
						at: "center top",
						offset: "0 0"
					}
				});
			},

			rate: function (source, action)
			{
				this.requestConnect(false, function ()
				{
					var count = $('.readerCount', 'label[for=' + source.id + ']').text();
					if (count == '-')
					{
						count = 0;
					}
					else
					{
						count = parseInt(count);
					}

					if ($(source).is(':checked'))
					{
						count++;
					}
					else
					{
						count--;
					}

					$('.readerCount', 'label[for=' + source.id + ']').text(count <= 0 ? '-' : count);

					var args = { comicId: this.options.comic.ComicId };
					$.ajax(
					{
						dataType: 'json',
						type: 'POST',
						url: this.options.baseHref + 'Comic/' + action,
						data: $.postify(args)
					});
				});
			},

			shareFb: function ()
			{
				FB.ui(
				{
					method: 'feed',
					name: this.options.comic.Title,
					description: this.options.comic.Description,
					picture: this.options.comic.FrameThumbUrl,
					link: this.options.comic.ReadUrl
				});
			}

		}, options);
	};
} (jQuery));