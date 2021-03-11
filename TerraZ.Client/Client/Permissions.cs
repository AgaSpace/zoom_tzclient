using System;

namespace TerraZ.Client
{
	// Token: 0x0200000F RID: 15
	public class Permissions
	{
		// Token: 0x06000049 RID: 73 RVA: 0x00002B4C File Offset: 0x00000D4C
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

		// Token: 0x0600004B RID: 75 RVA: 0x0000232A File Offset: 0x0000052A
		internal void SetPermissions(string Permissions)
		{
			this._permissions = Permissions;
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600004C RID: 76 RVA: 0x00002333 File Offset: 0x00000533
		public string ListPermissions
		{
			get
			{
				return this._permissions;
			}
		}

		// Token: 0x0400001E RID: 30
		private string _permissions = ",";
	}
}
