using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraZ.ServerSupport
{
    public class Group
    {
        public static Group Generate(string Name, string[] Permissions) => new Group() { Name = Name, Permissions = Permissions };
        private Group() { }

        public string Name { get; private set; }
        public string[] Permissions { get; private set; }
    }
}
