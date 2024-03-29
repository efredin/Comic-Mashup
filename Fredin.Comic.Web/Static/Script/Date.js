﻿Date.prototype.parse8601 = function(string) 
{
    var regexp = "([0-9]{4})(-([0-9]{2})(-([0-9]{2})(T([0-9]{2}):([0-9]{2})(:([0-9]{2})(\.([0-9]+))?)?(Z|(([-+])([0-9]{2}):([0-9]{2})))?)?)?)?";
    var d = string.match(new RegExp(regexp));

    var offset = 0;
    var date = new Date(d[1], 0, 1);

    if (d[3]) { date.setMonth(d[3] - 1); }
    if (d[5]) { date.setDate(d[5]); }
    if (d[7]) { date.setHours(d[7]); }
    if (d[8]) { date.setMinutes(d[8]); }
    if (d[10]) { date.setSeconds(d[10]); }
    if (d[12]) { date.setMilliseconds(Number("0." + d[12]) * 1000); }
    if (d[14]) {
        offset = (Number(d[16]) * 60) + Number(d[17]);
        offset *= ((d[15] == '-') ? 1 : -1);
    }

    offset -= date.getTimezoneOffset();
    time = (Number(date) + (offset * 60 * 1000));
    this.setTime(Number(time));
}
Date.prototype.to8601 = function(format, offset) 
{
    /* accepted values for the format [1-6]:
     1 Year:
       YYYY (eg 1997)
     2 Year and month:
       YYYY-MM (eg 1997-07)
     3 Complete date:
       YYYY-MM-DD (eg 1997-07-16)
     4 Complete date plus hours and minutes:
       YYYY-MM-DDThh:mmTZD (eg 1997-07-16T19:20+01:00)
     5 Complete date plus hours, minutes and seconds:
       YYYY-MM-DDThh:mm:ssTZD (eg 1997-07-16T19:20:30+01:00)
     6 Complete date plus hours, minutes, seconds and a decimal
       fraction of a second
       YYYY-MM-DDThh:mm:ss.sTZD (eg 1997-07-16T19:20:30.45+01:00)
    */
    if (!format) { var format = 6; }
    if (!offset) {
        var offset = 'Z';
        var date = this;
    } else {
        var d = offset.match(/([-+])([0-9]{2}):([0-9]{2})/);
        var offsetnum = (Number(d[2]) * 60) + Number(d[3]);
        offsetnum *= ((d[1] == '-') ? -1 : 1);
        var date = new Date(Number(Number(this) + (offsetnum * 60000)));
    }

    var zeropad = function (num) { return ((num < 10) ? '0' : '') + num; }

    var str = "";
    str += date.getUTCFullYear();
    if (format > 1) { str += "-" + zeropad(date.getUTCMonth() + 1); }
    if (format > 2) { str += "-" + zeropad(date.getUTCDate()); }
    if (format > 3) {
        str += "T" + zeropad(date.getUTCHours()) +
               ":" + zeropad(date.getUTCMinutes());
    }
    if (format > 5) {
        var secs = Number(date.getUTCSeconds() + "." +
                   ((date.getUTCMilliseconds() < 100) ? '0' : '') +
                   zeropad(date.getUTCMilliseconds()));
        str += ":" + zeropad(secs);
    } else if (format > 4) { str += ":" + zeropad(date.getUTCSeconds()); }

    if (format > 3) { str += offset; }
    return str;
}
Date.prototype.toContextual = function()
{
    var now = new Date();
    var duration = new Date(now.getTime() - this.getTime());
    var output = '';

    var year = duration.getUTCFullYear() - 1970; // 1970!
    var month = duration.getUTCMonth() - 1; // 1 based
    var day = duration.getUTCDate() - 1; // 1 based
    var hour = duration.getUTCHours(); // 0 based
    var minute = duration.getUTCMinutes(); // 0 based

    // year
    if (year > 1)
    {
        output = year + ' years ago';
    }
    else if (year > 0)
    {
        output = year + ' year ago';
    }

    // month
    else if (month > 1)
    {
        output = month + ' months ago';
    }
    else if (month > 0)
    {
        output = month + ' month ago';
    }

    // day
    else if (day > 1)
    {
        output = day + ' days ago';
    }
    else if (day > 0)
    {
        output = day + ' day ago';
    }

    // hour
    else if (hour > 1)
    {
        output = hour + ' hours ago';
    }
    else if (hour > 0)
    {
        output = hour + ' hour ago';
    }

    // minute
    else if (minute > 1)
    {
        output = minute + ' minutes ago';
    }
    else if (minute > 0)
    {
        output = minute + ' minute ago';
    }

    // less than 1 minute
    else
    {
        output = 'moments ago';
    }

    return output;
};
Date.parse8601 = function(date)
{
	var d = new Date();
	d.parse8601(date);
	return d;
}