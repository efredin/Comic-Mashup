/*
 * Build for comic specifically.
 * Dependant on jquery.ui and fg.menu
 */

(function ($)
{
	$.widget('ui.tabmenu',
    {
    	options:
		{
			selected: -1
		},

    	_create: function ()
    	{
    		this.element
                .addClass('ui-tabmenu ui-widget');
    		//.append('<ul class="ui-tabmenu-nav ui-helper-clearfix ui-widget-header ui-corner-all"></ul>');

    		$('ul:first', this.element)
                .addClass('ui-tabmenu-nav ui-helper-clearfix ui-widget-header ui-corner-all');

    		var mouseIn = function () { $(this).addClass('ui-state-hover'); };
    		var mouseOut = function () { $(this).removeClass('ui-state-hover'); };

    		$('ul:first > li', this.element)
                .addClass('ui-state-default ui-corner-top')
				.hover(mouseIn, mouseOut)
                .children('a')
                .each(function (index)
                {
                	var content = $(this).next('ul');

                	if (content.size() > 0)
                	{
                		content.hide();
                		$(this).menu(
                        {
                        	content: content.html(),
                        	showSpeed: 200
                        });
                	}
                });

    		this.select(this.options.selected);
    	},

    	_setOption: function (key, value)
    	{
    		if (key == "selected")
    		{
    			this.select(value);
    		}

    		$.Widget.prototype._setOption.apply(this, arguments);
    	},

    	select: function (index)
    	{
    		var items = $('ul:first > li', this.element);
    		items.removeClass('ui-tabmenu-selected ui-state-active');
    		if (index >= 0 && index < items.length)
    		{
    			items.eq(index).addClass('ui-tabmenu-selected ui-state-active');
    		}
    	}
    });
} (jQuery));
