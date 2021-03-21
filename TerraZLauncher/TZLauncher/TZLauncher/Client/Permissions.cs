using System;
using System.Collections.Generic;
using System.Linq;
using TZLauncher;

namespace TerraZ.Client
{
	public class Permissions
	{
		public bool HasPermission(string Permission)
		{
			if (Perms.Contains("*") || Perms.Contains("superadmin"))
				return true;

			if (Perms.Contains(Permission))
				return true;

			return false;
		}

		internal void SetPermissions(string Permissions)
		{
			this._permissions = Permissions;
		}

		public string ListPermissions => this._permissions;

		public List<string> Perms
        {
			get
            {
				return this._permissions.Split(',').ToList();
			}
        }

		private string _permissions = ",";
	}
}
