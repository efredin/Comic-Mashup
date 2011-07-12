(function ($)
{
	if (typeof Comic == "undefined") { Comic = {}; }

	Comic.Create = function (options)
	{
		Application.create(
		{
			options:
			{
				comic:
				{
					ComicId: null,
					Template: null,
					Bubbles: [],
					Photos: []
				},
				templates: [],
				effects: [],
				bubbleDirections: [],
				selectedBubbleDirection: null,
				selectedPhoto: null,
				activeFrame: 0,
				bubbleIndex: 0
			},

			init: function ()
			{
				var self = this;
				Application.prototype.init.apply(this);

				this.requestConnect(true, 'Comic Mashup needs to connect with your facebook account to retrieve your photos.', function ()
				{
					self.render();
					self.loadState();

					if (self.options.comic.ComicId != null)
					{
						self.loadComic();
					}
				});
			},

			render: function ()
			{
				var self = this;

				/* Template */
				$('#templateSelector').select2
				({
					multiple: false,
					data: self.options.templates,
					dataKey: 'TemplateId',
					dataValue: 'TemplateId',
					template: $('#templateTemplate'),
					emptyText: 'No templates found.',
					selected: function (event, data)
					{
						self.options.comic.Template = data.item;
						$('#wizardContent').wizard('step', '#stepCreate');
						$.bbq.pushState({ template: data.item.TemplateId });
						self.loadTemplate();
					}
				});

				// Bubble selector
				$('#bubbleSelector').select2
				({
					multiple: false,
					data: this.options.bubbleDirections,
					dataKey: 'TextBubbleDirectionId',
					dataValue: 'TextBubbleDirectionId',
					template: $('#bubbleTemplate'),
					emptyText: 'No text bubbles found.',
					selected: function (event, data)
					{
						self.options.selectedBubbleDirection = data.item;
					}
				});


				// Bubble modal dialog
				$('#bubbleDialog').dialog
				({
					autoOpen: false,
					buttons:
					{
						Add: function ()
						{
							$(this).dialog("close");

							self.addBubble(self.options.bubbleIndex++,
								{

									TextBubbleDirection: self.options.selectedBubbleDirection,
									Text: '',
									X: 0,
									Y: 0
								});
						},
						Cancel: function () { $(this).dialog("close"); }
					},
					closeOnEscape: true,
					draggable: false,
					modal: true,
					resizable: false,
					width: 448
				});


				// Photo modal dialog
				$('#photoDialog').dialog
				({
					autoOpen: false,
					buttons:
					{
						Add: function ()
						{
							$(this).dialog("close");
							self.renderPhoto();
						},
						Cancel: function () { $(this).dialog("close"); }
					},
					closeOnEscape: true,
					draggable: false,
					modal: true,
					resizable: false,
					width: 648
				});

				// Album selector
				$('#albumSelector').select2
				({
					multiple: false,
					data: [],
					dataKey: 'id',
					dataValue: 'id',
					template: $('#albumTemplate'),
					emptyText: 'No albums found.',
					selected: function (event, data)
					{
						// load photos
						$('#photoLoad').load('reset');
						self.loadPhotos(data.item.id, function (photos)
						{
							$('#photoLoad').load('complete');
							$('#photoSelector').select2('option', 'data', photos);
						});

						$('#photoWizard').wizard('step', '#stepPhoto');
					}
				});

				$('#albumLoad').load({ content: $('#albumContent') });
				this.loadAlbums(this.options.user.Uid, function (albums)
				{
					$('#albumSelector').select2('option', 'data', albums);
					$('#albumLoad').load('complete');
				});

				// Photo
				$('#photoSelector').select2
				({
					multiple: false,
					dataKey: 'id',
					dataValue: 'id',
					template: $('#photoTemplate'),
					emptyText: 'No photos found.',
					selected: function (event, data)
					{
						self.options.selectedPhoto = data.item;
						$('button:first', $('#photoDialog').next('.ui-dialog-buttonpane')).button('enable');
						$('#photoWizard').wizard('step', '#stepEffect');
					}
				});
				$('#photoLoad').load({ content: $('#photoContent') });

				// Effects
				$('#buttonsetIntensity').buttonset();
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

				// Photo wizard
				$('#photoWizard').wizard
				({
					width: 620,
					stateField: 'photoStep'
				});

				// Add bubble
				$('.ui-comic-bubbleAdd')
					.button({ icons: { primary: 'ui-icon-comment'} })
					.click(function ()
					{
						$('#bubbleDialog').dialog('open');
						$('#photoWizard').wizard('step', '#stepAlbum');
					});

				$('#renderDialog').dialog
				({
					autoOpen: false,
					buttons: {},
					closeOnEscape: false,
					draggable: false,
					modal: true,
					resizable: false,
					width: 300
				});

				/* Render */
				//$('#renderLoad').load({ content: '#renderContent', progressbar: true });

				/* Wizard */
				$('#wizardContent').wizard(
				{
					width: 734
				});

				/* Comic Publisher */
				$('#publishDialog').dialog(
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

				$('.ui-comic-publish')
					.button({ icons: { primary: 'ui-icon-signal-diag'} })
					.click(function () { self.showPublishComic(); });

				$('#buttonsetPrivacy').buttonset();

				$('#publishLoad').load({ content: '#publishForm', progressbar: true });

				$('#wizardContent').show();
			},

			loadState: function ()
			{
				var self = this;
				Application.prototype.loadState.apply(this);

				var templateId = $.bbq.getState('template', true);
				if (templateId != null && (!this.options.comic.Template || templateId != this.options.comic.Template.TemplateId))
				{
					$.each(this.options.templates, function (t, template)
					{
						if (templateId == template.TemplateId)
						{
							$('#templateSelector').select2('select', t);
						}
					});
				}
			},

			loadComic: function ()
			{
				var self = this;

				$.each(this.options.templates, function (t, template)
				{
					if (self.options.comic.Template.TemplateId == template.TemplateId)
					{
						$('#templateSelector').select2('select', t);
					}
				});

				$.each(this.options.comic.Photos, function (p, photo) { self.addPhoto(p, photo); });
				$.each(this.options.comic.Bubbles, function (b, bubble) { self.addBubble(b, bubble); });
			},

			loadTemplate: function ()
			{
				var self = this;

				$('.ui-comic-canvas')
				//.remove('.ui-comic-frame') bug in jquery
					.css('width', this.options.comic.Template.Width + 'px')
					.css('height', this.options.comic.Template.Height + 'px');

				$('.ui-comic-frame', '.ui-comic-canvas').remove();

				$.each(this.options.comic.Template.TemplateItems, function (i, item)
				{
					$('.ui-comic-canvas').append("<div class='ui-comic-frame'></div>");
					var frame = $('.ui-comic-frame:eq(' + i + ')')
						.css('width', item.Width + 'px')
						.css('height', item.Height + 'px')
						.css('left', item.X + 'px')
						.css('top', item.Y + 'px');

					if (self.options.comic.Photos[i] != null)
					{
						self.addPhoto(i, self.options.comic.Photos[i]);
					}
					else
					{
						// show photo add button
						frame.append("<div class='ui-comic-frameButton'><a>Select Photo</a></div>");
						$('.ui-comic-frameButton a', frame)
							.button({ icons: { primary: 'ui-icon-image'} })
							.click(function () { self.showAddPhoto(i); });
					}
				});
			},

			loadAlbums: function (uid, callback)
			{
				var self = this;
				$('#albumLoad').load('reset');

				FB.api('/' + uid + '/albums', {}, function (response)
				{
					if (!response || response.error)
					{
						self.error("Unable to retrieve albums from facebook.", response);
					}
					else
					{
						callback(response.data);
					}
				});
			},

			loadPhotos: function (albumId, callback)
			{
				var self = this;
				$('#photoLoad').load('reset');

				FB.api('/' + albumId + '/photos', {}, function (response)
				{
					if (!response || response.error)
					{
						self.error("Unable to retrieve photos from facebook.", response);
					}
					else
					{
						callback(response.data);
					}
				});
			},

			addBubble: function (index, bubble)
			{
				var self = this;

				this.options.comic.Bubbles[index] = bubble;

				$('.ui-comic-canvas').append("<textarea>" + bubble.Text + "</textarea>");

				var b = $('textarea', '.ui-comic-canvas').not('.ui-bubbleText')
					.data('index', index)
					.change(function ()
					{
						self.options.comic.Bubbles[$(this).data('index')].Text = $(this).val();
					})
					.bubble
					({
						allowDelete: true,
						bubble: bubble.TextBubbleDirection,
						onDelete: function (event, data)
						{
							self.options.comic.Bubbles.splice($(this).data('index'), 1);
						}
					})
					.focus()
					.parent()
					.css('left', bubble.X + 'px')
					.css('top', bubble.Y + 'px')
					.draggable
					({
						stop: function (event, ui)
						{
							var element = $('textarea', this);
							var b = self.options.comic.Bubbles[element.data('index')];
							if (b) // occasionally undefined when drag stop occurs out of frame
							{
								b.X = ui.position.left + parseInt(element.css('margin-left'));
								b.Y = ui.position.top + parseInt(element.css('margin-top'));
							}
						}
					});
			},

			showAddPhoto: function (index)
			{
				this.options.activeFrame = index;
				$('#photoWizard').wizard('step', '#stepAlbum');
				$('#photoDialog').dialog('open');
				$('button:first', $('#photoDialog').next('.ui-dialog-buttonpane')).button('disable');
			},

			addPhoto: function (index, photo)
			{
				var self = this;

				this.options.comic.Photos[index] = photo;
				var template = this.options.comic.Template.TemplateItems[index];

				// calculate fit image size
				var imageRatio = photo.Width / photo.Height;
				var scaleRatio = template.Width / template.Height;
				var fitWidth, fitHeight;
				if (imageRatio >= scaleRatio)
				{
					fitHeight = template.Height;
					fitWidth = imageRatio * template.Height;
				}
				else
				{
					fitWidth = template.Width;
					fitHeight = photo.Height / photo.Width * template.Width;
				}

				// calculate crop offset
				var offsetX = (fitWidth - template.Width) / 2 * -1;
				var offsetY = (fitHeight - template.Height) / 2 * -1;

				var frame = $('.ui-comic-frame:eq(' + index + ')')
					.empty()
					.append("<img class='' src='" + photo.ImageUrl + "' alt='' width='" + fitWidth + "' height='" + fitHeight + "' />")
					.append("<div class='ui-comic-frameButton'><a>Change Photo</a></div>");

				frame.children('img')
					.css('left', offsetX + 'px')
					.css('top', offsetY + 'px');

				$('a', frame)
					.button
					({
						text: false,
						icons: { primary: 'ui-icon-image' }
					})
					.click(function () { self.showAddPhoto(index); });
			},

			renderPhoto: function ()
			{
				var self = this;
				var task = null;

				$('#renderDialog').dialog('open');

				// Translate to request params
				var data =
				{
					effect: $('#effectSelector').val(),
					photoSource: this.options.selectedPhoto.source,
					intensity: $('#buttonsetIntensity :checked').val()
				};

				var timeout = 1000 * 20; // 20 second timeout!
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
						$('#renderDialog').dialog('close');
					}

					// get progress from server
					$.ajax
					({
						dataType: 'json',
						type: 'GET',
						url: self.options.baseHref + 'Comic/PhotoRenderProgress',
						data: { taskId: task.TaskId },
						success: function (data, textStatus, request)
						{
							// Update render progress ui
							if (data)
							{
								task = data;
								if (task.Status == 2)
								{
									// Render complete
									window.clearInterval(progressInterval);
									self.addPhoto(self.options.activeFrame, task.Photo);
									$('#renderDialog').dialog('close');
								}
								else if (task.Status == 3)
								{
									window.clearInterval(progressInterval);
									self.error('Unable to render your photo. Please try again');
									$('#renderDialog').dialog('close');
								}
							}
						},
						error: function (xhr, textStatus, x)
						{
							// Prevent application default error handler from firing
						}
					});
				}

				$.ajax(
				{
					dataType: 'json',
					type: 'POST',
					url: this.options.baseHref + 'Comic/QueuePhotoRender',
					data: $.postify(data),
					success: function (data, textStatus, request)
					{
						if (data)
						{
							task = data;
							console.debug("Queued photo render task");
							console.debug(task);

							progressCallback();
							progressInterval = window.setInterval(progressCallback, progressFrequency);
						}
					}
				});
			},

			showPublishComic: function ()
			{
				// Set title & description
				$('#comicTitle').val(this.options.user.Name != '' ? this.options.user.Name + "'s Comic Mashup" : 'My Comic Mashup');

				var description = '';
				this.options.comic.Bubbles.each(function (index, bubble)
				{
					if (description == '') description = bubble.Text;
				});
				$('#comicDescription').val(description);

				$('#publishLoad').load("complete");
				$('#publishDialog').dialog('open');
			},

			publishComic: function ()
			{
				var self = this;
				var task = null;
				var load = $('#publishLoad').load("reset");

				// Translate to request params
				var data =
				{
					remixComicId: this.options.comic.ComicId != null && this.options.comic.ComicId > 0 ? this.options.comic.ComicId : null,
					templateId: this.options.comic.Template.TemplateId,
					frames: [],
					bubbles: []
				};

				this.options.comic.Photos.each(function (index, photo)
				{
					data.frames.push({ PhotoId: photo.PhotoId });
				});

				this.options.comic.Bubbles.each(function (index, bubble)
				{
					data.bubbles.push({ TextBubbleDirectionId: bubble.TextBubbleDirection.TextBubbleDirectionId, Text: bubble.Text, X: bubble.X, Y: bubble.Y });
				});

				var publish = function ()
				{
					var publishData =
					{
						comicId: self.options.comic.ComicId,
						title: $('#comicTitle').val(),
						description: $('#comicDescription').val(),
						isPrivate: $('[name=optionPrivacy]:checked').val() === 'Friends'
					};

					console.log('Publishing comic ' + self.options.comic.ComicId);
					$.ajax(
					{
						dataType: 'json',
						type: 'POST',
						url: self.options.baseHref + 'Comic/Publish',
						data: $.postify(publishData),
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
									link: self.options.comic.ReadUrl,
									actions:
									[
									//{ name: 'Read', link: self.options.comic.ReadUrl },
										{name: 'Remix', link: self.options.comic.RemixUrl }
									]
								}, function (response)
								{
									document.location = data.ReadUrl;
								});
							}
							else
							{
								self.error('Unable to publish your comic. Please try again later.');
							}
						}
					});
				};

				var timeout = 1000 * 60 * 2; // 2 minute timeout!
				var progressFrequency = 1000 * 5; // check every 5 seconds
				var progressTime = 0;
				var progressInterval = null;
				var progressCallback = function ()
				{
					progressTime += progressFrequency;
					if (progressTime > timeout)
					{
						window.clearInterval(progressInterval);
						self.error("Unable to publish your comic. Please try again.");
					}

					// get progress from server
					$.ajax
					({
						dataType: 'json',
						type: 'GET',
						url: self.options.baseHref + 'Comic/RenderProgress',
						data: { taskId: task.TaskId },
						success: function (data, textStatus, request)
						{
							// Update render progress ui
							if (data)
							{
								task = data;
								$('#publishLoad').load('progress', task.CompletedOperations / task.TotalOperations * 100);
								if (task.Status == 2)
								{
									// Render complete
									window.clearInterval(progressInterval);
									self.options.comic = task.Comic;

									// Publish
									publish();
								}
								else if (task.Status == 3)
								{
									window.clearInterval(progressInterval);
									$('#publishLoad').load("complete");
									$('#publishDialog').dialog('close');
									self.error('Unable to publish your comic. Please try again');
								}
							}
						},
						error: function (xhr, textStatus, x)
						{
							// Prevent application default error handler from firing
						}
					});
				}

				$.ajax(
				{
					dataType: 'json',
					type: 'POST',
					url: this.options.baseHref + 'Comic/QueueRenderAdvanced',
					data: $.postify(data),
					success: function (data, textStatus, request)
					{
						if (data)
						{
							task = data;
							console.debug("Queued comic publish task");
							console.debug(task);

							// Callback to check on render progress
							progressCallback();
							progressInterval = window.setInterval(progressCallback, progressFrequency);
						}
					}
				});
			}

		}, options);
	};
} (jQuery));