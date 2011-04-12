﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fredin.Comic.Render
{
	[Serializable]
	public class ProfileTask : Task
	{
		public long OwnerUid { get; set; }
		public string FacebookToken { get; set; }
		public ComicEffectType Effect { get; set; }
		public int Intensity { get; set; }
		public string StorageKey { get; set; }
	}
}
