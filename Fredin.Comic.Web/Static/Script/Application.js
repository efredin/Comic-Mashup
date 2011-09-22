(function ($)
{
	Application = function ()
	{
		this.initComplete = false;
		this.connected = false;
		this.requestConnectDialog = null;
		this.connectCallback = null;
		this.errrorDialog = null;
	};

	Application.instance = null;
	Application.create = function (prototype, options)
	{
		var app = $.extend(true, new Application(), prototype);
		Application.instance = app;
		$.extend(true, app.options, options);

		$(document).ready(function ()
		{
			app.init();
			app.loadState();
		});
		//$(window).trigger('hashchange');
	};

	Application.prototype =
    {
    	options:
        {
        	appId: null,
        	permissions: '',
        	cookieSupport: true,
        	useXfbml: false,
        	user: null,
        	menuSelected: -1,
        	baseHref: '/',
        	facebookBaseHref: '/',
        	requireConnect: false,
        	themeBase: '',
        	fbCanvas: false
        },

    	init: function ()
    	{
    		var self = this;
    		console.info('Initializing application');
    		console.warn(this.options);
    		if (this.options.fbCanvas === true)
    		{
    			// Check for presense of parent facebook frame.  If its missing we aren't actually on facebook and should therefore
    			// reload the page with the fbcanvas=false parameter
    			try
    			{
    				console.log(top.location);
    				if (top.location.href == window.location.href)
    				{
    					console.log('matches...');
    					var href = window.location.href;
    					if (href.indexOf('?') <= 0)
    					{
    						href += '?';
    					}
    					href += '&fbcanvas=false';
    					window.location = href;
    				}
    			}
    			catch (x)
    			{
    				console.log(x);
    				// Meant to catch cross-domain scripting exception. Indicates the parent frame is in fact facebook
    			}
    		}

    		// Ajax defaults
    		$.ajaxSetup({ error: function (request, status, x) { self.error("An error has occured.", request, status, x); } });

    		// Template config
    		$.jqotetag('#');

    		var sessionChangeCallback = function (response)
    		{
    			self.initComplete = true;
    			console.debug(response);
    			if (response.session && response.status == "connected")
    			{
    				self.onConnect(response.session.uid);
    			}
    			else
    			{
    				self.onDisconnect();
    			}
    		}

    		// Initialize fb
    		FB.init({ appId: this.options.appId, status: false, cookie: this.options.cookieSupport, xfbml: this.options.useXfbml });
    		// Hook into facebook auth status changes
    		FB.Event.subscribe('auth.sessionChange', sessionChangeCallback);
    		FB.getLoginStatus(sessionChangeCallback);

    		if (this.options.fbCanvas === true)
    		{
    			FB.Canvas.setSize();
    		}

    		// Auth button click handlers
    		$('.button-fbLogin').click(function () { self.connect(); });
    		$('.button-fbLogout').click(function () { self.disconnect(); });

    		// Require connect modal dialog
    		this.requestConnectDialog = $('#dialog-requestConnect').dialog(
			{
				autoOpen: false,
				buttons: { Cancel: function () { $(this).dialog("close"); } },
				closeOnEscape: false,
				draggable: false,
				modal: true,
				resizable: false,
				close: function () { self.onConnectDialogClose(); }
			});

    		// Error feedback dialog
    		this.errorDialog = $('#dialog-error').dialog(
			{
				autoOpen: false,
				buttons: { Ok: function () { $(this).dialog('close'); } },
				closeOnEscape: true,
				draggable: false,
				modal: true,
				resizable: false
			});

    		// Buttons
    		//$('.ui-button').button();
    		//$(':checkbox').input();
    		//$(':radio').input();

    		// Menu
    		var menuOpen = function ()
    		{
    			$(this).addClass('active');
    			$('div:first', this).show();
    		};

    		var menuClose = function ()
    		{
    			$(this).removeClass('active');
    			$('div:first', this).hide();
    		};

    		$('.menu > ul > li')
				.hoverIntent
				({
					sensitivity: 20,
					over: menuOpen,
					out: menuClose,
					timeout: 200
				})
				.each(function ()
				{
					if ($('div', this).size() > 0)
					{
						$('a:first', this).append('<span class="ui-icon ui-icon-triangle-1-s"></span>');
					}
				});

    		// Search
    		$('#search-text').watermark('Comic Search');

    		// Styling for even /odd list items
    		$('ul,ol').each(function ()
    		{
    			$('li:odd', this).addClass('odd');
    		});

    		// Tooltips
    		$('[title]').not('.ui-button,div').tipsy();

    		// Make links ui state awesome
    		$('a.ui-state-default').hover(function () { $(this).addClass('ui-state-hover'); }, function () { $(this).removeClass('ui-state-hover'); });

    		// Application state
    		$(window).bind('hashchange', function () { self.loadState(); });
    	},

    	connect: function ()
    	{
    		FB.login(null, { perms: this.options.permissions });
    	},

    	disconnect: function ()
    	{
    		FB.logout(null);
    	},

    	onConnect: function (uid)
    	{
    		console.info('User %s connected', uid);

    		var self = this;
    		this.connected = true;

    		// Load user
    		if (this.options.user == null || this.options.user.uid != uid)
    		{
    			$.ajax({ async: false, url: self.options.baseHref + 'User/Me', success: function (data, textStatus, request) { self.loadUser(data); } });
    		}

    		// Resolve any outstanding connect callbacks
    		if (this.connectCallback)
    		{
    			this.connectCallback.call(this);
    		}

    		// Update UI
    		this.requestConnectDialog.dialog("close");
    		this.toggleConnectUi();
    	},

    	onDisconnect: function ()
    	{
    		console.info('User disconnected');

    		this.connected = false;

    		// Unload user
    		if (this.options.user != null)
    		{
    			this.loadUser(null);

    			// Kill the server session
    			$.ajax({ url: this.options.baseHref + '/User/Logout' });
    		}

    		// Update UI
    		this.toggleConnectUi();

    		if (this.options.requireConnect)
    		{
    			this.requestConnect();
    		}
    	},

    	requestConnect: function (required, message, callback)
    	{
    		var self = this;
    		this.options.requireConnect = required != null ? required : true;

    		$('#dialog-requestConnect-message').html((message && message != '') ? message : 'Please login to continue.');

    		var initCallback = function ()
    		{
    			if (self.connected)
    			{
    				callback.call(this);
    			}
    			else
    			{
    				// Modal dialog requiring connect
    				self.connectCallback = callback;
    				self.requestConnectDialog.dialog("open");
    			}
    		}

    		if (!this.initComplete)
    		{
    			var initInterval;
    			initInterval = window.setInterval(function ()
    			{
    				if (self.initComplete)
    				{
    					window.clearInterval(initInterval);
    					initCallback();
    				}
    			}, 250);
    		}
    		else
    		{
    			initCallback();
    		}
    	},

    	onConnectDialogClose: function ()
    	{
    		if (!this.connected && this.options.requireConnect)
    		{
    			document.location = this.options.baseHref + 'User/Login';
    		}
    	},

    	requirePermission: function (permission, callback)
    	{
    		// regenerate list of permissions
    		var requestPerms = permission.split(',');
    		var perms = this.options.permissions.split(',');
    		for (var p = 0; p < requestPerms.length; p++)
    		{
    			if (!perms.contains(requestPerms[p]))
    			{
    				perms.push(requestPerms[p]);
    			}
    		}
    		var permission = perms.join(',');
    		if (this.options.permissions != permission)
    		{
    			// request new permissions if changed
    			this.options.permissions = permission;
    			FB.login(callback, { perms: this.options.permissions });
    		}
    	},

    	toggleConnectUi: function ()
    	{
    		if (this.connected)
    		{
    			$('#buttonLogin').hide();
    			$('#buttonLogout').css('display', 'inline-block');
    		}
    		else
    		{
    			$('#buttonLogout').css('display', 'none');
    			$('#buttonLogin').show();
    		}
    	},

    	loadUser: function (user)
    	{
    		this.options.user = user;

    		if (user == null)
    		{
    			console.info('Clearing user');
    			$('#login-name').html('You are not logged in');
    			$('#login-photo').empty();
    		}
    		else
    		{
    			console.info(user);
    			$('#login-name').html(user.Name);
    			$('#login-photo').html('<img src="http://graph.facebook.com/' + user.Uid + '/picture/" alt="' + user.Nickname + '" />');
    		}
    	},

    	loadState: function ()
    	{
    	},

    	changeTheme: function (themeName)
    	{
    		console.log('Changing theme to ' + themeName);
    		$('#cssTheme').attr('href', this.options.themeBase + '/' + themeName + '/jquery.ui.theme.min.css');

    		$.ajax
			({
				dataType: 'json',
				url: this.options.baseHref + 'User/ChangeTheme',
				data: { theme: themeName }
			});
    	},

    	error: function (message)
    	{
    		console.debug(arguments);
    		$('#dialog-error-message').html(message);
    		this.errorDialog.dialog('open');
    		$.ajax
			({
				dataType: 'json',
				type: 'POST',
				url: this.options.baseHref + 'Error/LogError',
				data: $.postify({ x: message }),
				error: function (xhr, textStatus, x){ } // If recording error fails give up. don't want to create an super-loop
			});
    	},

    	warn: function ()
    	{
    		//TODO: Display feedback to user
    		console.debug(arguments);
    	}
    };

} (jQuery));


// stub in logging when unavailable
if (typeof console === "undefined")
{
	console =
    {
    	log: function () { },
    	debug: function () { },
    	info: function () { },
    	warn: function () { },
    	error: function () { },
    	group: function () { },
    	groupEnd: function () { },
		trace: function() { }
    };
}