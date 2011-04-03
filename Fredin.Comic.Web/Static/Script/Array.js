Array.prototype.remove = function(item)
{
	for(var i = 0; i < this.length; i++)
	{
		if(this[i] == item)
		{
			this.splice(i, 1);
			break;
		}
	}
}
Array.prototype.each = function(callback)
{
	for(var i = 0; i < this.length; i++)
	{
		callback.call(this, i, this[i]);
	}
}
Array.prototype.findIndex = function (callback)
{
	for(var i = 0; i < this.length; i++)
	{
		if(callback(this[i]))
		{
			return i;
		}
	}
	return -1;
}
Array.prototype.contains = function (item)
{
	for(var i = 0; i < this.length; i++)
	{
		if(this[i] === item)
		{
			return true;
		}
	}
	return false;
}