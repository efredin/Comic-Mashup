using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fredin.Comic
{
	[Serializable()]
	public class Task
	{
		public virtual Guid TaskId { get; set; }
		public virtual TaskStatus Status { get; set; }
	}
}
