using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Mvc;

namespace Fredin.Comic.Web.Models
{
	//[Bind(Include = "Uid,Email,Name,Nickname,ProfileLink")]
	[MetadataType(typeof(UserMetadata))]
	public partial class User
	{
	}

	public class UserMetadata
	{

	}
}