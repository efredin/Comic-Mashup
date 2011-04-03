﻿(function ($)
{
	if (typeof Comic == "undefined") { Comic = {}; }

	Comic.CreateWizard = function (options)
	{
		Application.create(
		{
			options:
			{
				step: 0,
				templates: [],
				effects: [],
				feed: [],
				friends: [],
				selectedStory: null,
				selectedComments: [],
				comic: null,
				taskBaseHref: null
			},

			init: function ()
			{
				var self = this;
				Application.prototype.init.apply(this);

				$('#wizardLoad').load({ content: '#wizardContent', progressbar: true });
				$('#publishComic').hide();

				this.requestConnect(true, function ()
				{
					var load = $('#wizardLoad').load('progress', 25);

					self.loadFriends(function ()
					{
						load.load('progress', 50);
						self.loadFeed(function ()
						{
							load.load('progress', 75);
							self.render();
							self.loadState();
							load.load('progress', 100);
							load.load("complete");
						});
					});
				});
			},

			render: function ()
			{
				var self = this;

				/* Story */
				$('#storySelector').select2(
				{
					multiple: false,
					data: self.options.feed,
					template: $('#storyTemplate'),
					dataKey: 'id',
					dataValue: 'message',
					emptyText: 'No messages found.',
					selected: function (event, data)
					{
						self.options.selectedStory = data.item;
						$.bbq.pushState({ story: data.item.id });

						self.options.selectedComments = [];

						var commentCallback = function (comments)
						{
							data.item.comments = comments;
							$('#commentSelector').select2('option', 'data', comments);
						};
						if (!data.item.comments || (data.item.comments.count && data.item.comments.count != data.item.comments.length))
						{
							// load comments
							self.loadComments(data.item.id, commentCallback);
						}
						else
						{
							commentCallback(data.item.comments ? data.item.comments.data : []);
						}

						$('#wizardContent').wizard('step', '#stepComment');
					}
				});

				if ($('.feed-suggested').size() <= 2)
				{
					$('#storyFilterSuggest').val('.feed-suggested');
				}
				var filterChange = function (e)
				{
					if ($(this).is(':checked'))
					{
						$('#storySelector').select2("filter", $(this).val());
					}
				};
				$('#storyFilter :radio').change(filterChange);
				$('#storyFilterSuggest').attr('checked', true).change();

				/* Comment */
				var updateComments = function ()
				{
					// update template suggested filter
					var count = self.options.selectedComments.length + 1;

					$('#templateFilterSuggest').val('.template-size' + count);
					if ($('.template-size' + count).size() === 0)
					{
						$('#templateFilterAll').attr('checked', true).change();
					}
					else
					{
						$('#templateFilterSuggest').attr('checked', true).change();
					}
				}

				$('#commentLoad').load({ content: $('#commentContent') });
				$('#commentSelector').select2(
				{
					multiple: true,
					template: $('#commentTemplate'),
					dataKey: 'id',
					dataValue: 'message',
					emptyText: 'No comments found.',
					selected: function (event, data)
					{
						self.options.selectedComments.push(data.item);
						updateComments();

						// hop to next step when all comments are selected
						if (self.options.selectedStory.comments && self.options.selectedComments.length >= self.options.selectedStory.comments.length)
						{
							$('#wizardContent').wizard('step', '#stepTemplate');
						}
					},
					unselected: function (event, data)
					{
						self.options.selectedComments.remove(data.item);
						updateComments();
					}
				});

				/* Template */
				$('#templateSelector').select2(
				{
					multiple: false,
					data: self.options.templates,
					dataKey: 'TemplateId',
					dataValue: 'TemplateId',
					template: $('#templateTemplate'),
					emptyText: 'No templates found.',
					selected: function (event, data)
					{
						$('#wizardContent').wizard('step', '#stepOption');
					}
				});

				var templateFilterChange = function (e)
				{
					if ($(this).is(':checked'))
					{
						$('#templateSelector').select2("filter", $(this).val());
					}
				};
				$('#templateFilter :radio').change(templateFilterChange);

				/* Render Options */
				$('#buttonsetPhotoSource').buttonset();
				$('.ui-optionset').optionset();

				$('#effectSelector').select2(
				{
					multiple: false,
					data: self.options.effects,
					dataKey: 'EffectId',
					dataValue: 'Title',
					template: $('#templateEffect'),
					emptyText: 'No effects found.'
				});
				$('#effectSelector').select2('select', 0);

				/* Render */
				$('#renderLoad').load({ content: '#renderContent', progressbar: true });

				/* Validation */
				var validator = $('form', '#content').validate(
				{
					debug: true,
					errorClass: 'ui-state-highlight',
					rules:
					{
						storySelector: { required: true },
						templateSelector: { required: true }
					},
					messages:
					{
						storySelector: { required: 'Select a story to continue.' },
						templateSelector: { required: 'Select a template to continue.' }
					}
				});

				/* Wizard */
				$('#wizardContent').wizard(
				{
					validator: validator,
					width: 764,
					stepActive: function (event, data)
					{
						if (data && data.step == 3)
						{
							// issues with photo source buttonset being disabled
							$('#buttonsetPhoto').buttonset('enable');
						}
						if (data && data.step == 4)
						{
							// Render
							self.renderComic();
						}
					}
				});

				/* Comic Publisher */
				$('#publishComic').dialog(
				{
					autoOpen: false,
					buttons:
					{
						Publish: function () { self.publishComic(); },
						Cancel: function () { $(this).dialog("close"); }
					},
					closeOnEscape: true,
					draggable: false,
					modal: true,
					resizable: false,
					width: 500
				});

				$('#buttonsetPrivacy').buttonset();

				$('#buttonPublish').button({ icons: { primary: 'ui-icon-signal-diag'} })
					.click(function () { self.showPublishComic(); });

				$('#buttonRefresh').button({ icons: { primary: 'ui-icon-refresh'} })
					.click(function () { self.renderComic(); });
			},

			loadFeed: function (callback)
			{
				var self = this;

				// Get news feed data from facebook graph
				FB.api('/me/feed', { limit: 50 }, function (response)
				{
					if (!response || response.error)
					{
						self.error("Unable to retrieve feed from facebook.", response);
					}
					else
					{
						self.options.feed = [];

						// Generate recommended list - user generated or many comments
						for (var i = 0; i < response.data.length; i++)
						{
							var item = response.data[i];

							// Only include items from user or friends which have an actual message
							if (item.from && item.from.id == self.options.user.Uid && item.message && item.message.trim() != '')
							{
								self.options.feed.push(item);
							}
							else continue;

							item.css = "";
							if (item.comments && item.comments.count >= 2)
							{
								item.css += "feed-suggested";
							}
						}
					}

					if ($.isFunction(callback)) callback();
				});
			},

			loadFriends: function (callback)
			{
				var self = this;

				// Get news feed data from facebook graph
				FB.api('/me/friends', { limit: 5000 }, function (response)
				{
					if (!response || response.error)
					{
						self.error("Unable to retrieve friends from facebook.", response);
					}
					else
					{
						self.options.friends = response.data;
					}

					if ($.isFunction(callback)) callback();
				});
			},

			loadComments: function (id, callback)
			{
				var self = this;
				$('#commentLoad').load('reset');

				FB.api('/' + id + '/comments', {}, function (response)
				{
					if (!response || response.error)
					{
						self.error("Unable to retrieve comments from facebook.", response);
					}
					else
					{
						callback(response.data);
						$('#commentLoad').load('complete');
					}
				});
			},

			loadState: function ()
			{
				// Defer state load until after load has completed...
				if (this._feedLoaded)
				{
					Application.prototype.loadState.apply(this);

					var story = $.bbq.getState('story', true);
					if (story != null && (!this.options.selectedStory || story != this.options.selectedStory.id))
					{
						for (var i = 0; i < this.options.feed.length; i++)
						{
							if (this.options.feed[i].id == story)
							{
								$('#storySelector').select2('select', i);
								break;
							}
						}
					}
				}
			},

			renderComic: function ()
			{
				var self = this;
				var load = $('#renderLoad').load("reset");

				// Request render progress from server on an interval
				var taskId = $.Guid.New();
				var progressCallback = function ()
				{
					if ($.Guid.IsEmpty(taskId))
					{
						// render is complete - clear interval
						window.clearInterval(progressInterval);
					}
					else
					{
						// get progress from server
						$.ajax
						({
							dataType: 'json',
							type: 'GET',
							async: true,
							timeout: 10000, // cancel request if another is about to begin
							url: self.options.taskBaseHref + taskId,
							success: function (data, textStatus, request)
							{
								// Update render progress ui
								if (data)
								{
									$('#renderLoad').load('progress', data.Progress / data.Operations);
								}
							},
							error: function (xhr, textStatus, x)
							{
								// Prevent application default error handler from firing
							}
						});
					}
				}
				var progressInterval = window.setInterval(progressCallback, 10000);

				// Translate to request params
				var data =
				{
					taskId: taskId,
					effect: $('#effectSelector').val(),
					photoSource: $('[name=optionPhoto]:checked').val(),
					templateId: $('#templateSelector').val(),
					frames: [{ message: this.options.selectedStory.message, id: this.options.selectedStory.from.id}]
				};

				this.options.selectedComments.each(function (index, comment)
				{
					data.frames.push({ message: comment.message, id: comment.from.id });
				});

				console.log(data);
				$.ajax(
				{
					dataType: 'json',
					type: 'POST',
					async: true,
					url: this.options.baseHref + 'Comic/RenderWizard',
					data: $.postify(data),
					success: function (data, textStatus, request)
					{
						if (data)
						{
							self.options.comic = data;
							console.log('Created comic ' + data.ComicId);
							console.log(data);
							$('#renderComic').html('<img src="' + data.ComicUrl + '" alt="' + data.Title + '" />');

							taskId = $.Guid.Empty();
							$('#renderLoad').load("complete");
						}
					}
				});

				progressCallback();
			},

			showPublishComic: function ()
			{
				// Thumb image
				$('#publishImage').html('<img src="' + this.options.comic.FrameThumbUrl + '" alt="' + this.options.comic.Title + '" />');

				// Set title & description
				$('#comicTitle').val(this.options.user.Name != '' ? this.options.user.Name + "'s Comic" : 'My Comic');
				$('#comicDescription').val(this.options.selectedStory.message);

				$('#publishComic').dialog('open');
			},

			publishComic: function ()
			{
				var self = this;

				if (!this.publishComicPending)
				{
					this.publishComicPending = true;
					var data =
					{
						comicId: this.options.comic.ComicId,
						title: $('#comicTitle').val(),
						description: $('#comicDescription').val(),
						isPrivate: $('[name=optionPrivacy]:checked').val() === 'Friends'
					};

					console.log('Publishing comic ' + this.options.comic.ComicId);
					$.ajax(
					{
						dataType: 'json',
						type: 'POST',
						url: this.options.baseHref + 'Comic/PublishWizard',
						data: $.postify(data),
						success: function (data, textStatus, request)
						{
							if (data)
							{
								self.options.comic = data;
								console.log('Published comic ' + data.ComicId);

								// Prompt for facebook wall post
								FB.ui(
								{
									method: 'feed',
									name: self.options.comic.Title,
									description: self.options.comic.Description,
									picture: self.options.comic.FrameThumbUrl,
									link: self.options.comic.ReadUrl
								}, function (response)
								{
									window.location = data.ReadUrl;
								});
							}
							else
							{
								self.error('Unable to publish your comic. Please try again later.');
							}
							this.publishComicPending = false;
						}
					});
				}
			}

		}, options);
	};
} (jQuery));