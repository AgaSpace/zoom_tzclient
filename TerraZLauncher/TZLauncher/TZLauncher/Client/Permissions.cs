using System;

namespace TerraZ.Client
{
	// Token: 0x0200000F RID: 15
	public class Permissions
	{
		public bool HasPermission(string Permission)
		{
			string[] array = this._permissions.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				if (Permission == array[i])
				{
					return true;
				}
			}
			return false;
		}

		internal void SetPermissions(string Permissions)
		{
			this._permissions = Permissions;
		}

		public string ListPermissions => this._permissions;
		private string _permissions = ",";
	}
}
