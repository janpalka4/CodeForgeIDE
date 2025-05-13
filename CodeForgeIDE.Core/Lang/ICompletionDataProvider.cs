using AvaloniaEdit.CodeCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeForgeIDE.Core.Lang
{
    public interface ICompletionDataProvider
    {
        public List<ICompletionData> GetCompletionData(string path, int offset);
    }
}
