using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeForgeIDE.Core.Plugins
{
    public interface IFileScopedProvider
    {
        public bool ShouldBeUsed(string path);
    }
}
