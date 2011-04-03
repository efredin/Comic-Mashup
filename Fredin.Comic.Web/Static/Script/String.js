String.prototype.urlEncode = function()
{
    var encoded = escape(this);
    encoded = encoded.replace("+", "%2B");
    encoded = encoded.replace("/", "%2F");
    return encoded;
};
String.prototype.trim = function()
{
	return this.replace(/^\s*/, "").replace(/\s*$/, "");
}
String.prototype.wordSubstr = function(start, length, hint)
{
	if(!hint) hint = '';
	var string = this.substr(start);
	return string.length >= length ? string.substr(0, length).replace(/[^\s]*$/g, '') + hint : string;
}