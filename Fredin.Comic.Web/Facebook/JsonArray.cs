// Code From: http://github.com/NikhilK/dynamicrest
// For more about this code view Nilhil's blog post: 
// http://www.nikhilk.net/CSharp-Dynamic-Programming-REST-Services.aspx
// Used with permission of Nikhil Kothari. 
// Updated to be compatible with .Net 4.0 RTM on 5/17/2010

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;

namespace Fredin.Comic.Web.Facebook
{

	public sealed class JsonArray : DynamicObject, ICollection<object>, ICollection
	{

		private List<object> _members;

		public JsonArray()
		{
			_members = new List<object>();
		}

		public JsonArray(object o)
			: this()
		{
			_members.Add(o);
		}

		public JsonArray(object o1, object o2)
			: this()
		{
			_members.Add(o1);
			_members.Add(o2);
		}

		public JsonArray(params object[] objects)
			: this()
		{
			_members.AddRange(objects);
		}

		public override bool TryConvert(ConvertBinder binder, out object result)
		{
			Type targetType = binder.Type;

			if ((targetType == typeof(IEnumerable)) ||
				(targetType == typeof(IEnumerable<object>)) ||
				(targetType == typeof(ICollection<object>)) ||
				(targetType == typeof(ICollection)))
			{
				result = this;
				return true;
			}
			return base.TryConvert(binder, out result);
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			if (String.Compare(binder.Name, "Item", StringComparison.Ordinal) == 0)
			{
				if (args.Length == 1)
				{
					result = _members[System.Convert.ToInt32(args[0])];
					return true;
				}
				else if (args.Length == 2)
				{
					_members[System.Convert.ToInt32(args[0])] = args[1];
					result = null;
					return true;
				}
			}
			else if (String.Compare(binder.Name, "Add", StringComparison.Ordinal) == 0)
			{
				_members.Add(args[0]);
				result = null;
				return true;
			}
			else if (String.Compare(binder.Name, "Insert", StringComparison.Ordinal) == 0)
			{
				_members.Insert(System.Convert.ToInt32(args[0]), args[1]);
				result = null;
				return true;
			}
			else if (String.Compare(binder.Name, "IndexOf", StringComparison.Ordinal) == 0)
			{
				result = _members.IndexOf(args[0]);
				return true;
			}
			else if (String.Compare(binder.Name, "Clear", StringComparison.Ordinal) == 0)
			{
				_members.Clear();
				result = null;
				return true;
			}
			else if (String.Compare(binder.Name, "Remove", StringComparison.Ordinal) == 0)
			{
				result = _members.Remove(args[0]);
				return true;
			}
			else if (String.Compare(binder.Name, "RemoveAt", StringComparison.Ordinal) == 0)
			{
				_members.RemoveAt(System.Convert.ToInt32(args[0]));
				result = null;
				return true;
			}
			return base.TryInvokeMember(binder, args, out result);
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			if (String.Compare("Length", binder.Name, StringComparison.Ordinal) == 0)
			{
				result = _members.Count;
				return true;
			}
			return base.TryGetMember(binder, out result);
		}

		#region Implementation of IEnumerable
		IEnumerator IEnumerable.GetEnumerator()
		{
			return _members.GetEnumerator();
		}
		#endregion

		#region Implementation of IEnumerable<object>
		IEnumerator<object> IEnumerable<object>.GetEnumerator()
		{
			return _members.GetEnumerator();
		}
		#endregion

		#region Implementation of ICollection
		int ICollection.Count
		{
			get
			{
				return _members.Count;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Implementation of ICollection<object>
		int ICollection<object>.Count
		{
			get
			{
				return _members.Count;
			}
		}

		bool ICollection<object>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		void ICollection<object>.Add(object item)
		{
			((ICollection<object>)_members).Add(item);
		}

		void ICollection<object>.Clear()
		{
			((ICollection<object>)_members).Clear();
		}

		bool ICollection<object>.Contains(object item)
		{
			return ((ICollection<object>)_members).Contains(item);
		}

		void ICollection<object>.CopyTo(object[] array, int arrayIndex)
		{
			((ICollection<object>)_members).CopyTo(array, arrayIndex);
		}

		bool ICollection<object>.Remove(object item)
		{
			return ((ICollection<object>)_members).Remove(item);
		}
		#endregion
	}
}
