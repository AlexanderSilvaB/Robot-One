using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Scripts.GameLanguage
{
    public class CodeResults
    {
        public string Code { get; internal set; }
        public List<string> Errors { get; internal set; }
        public Assembly CompiledAssembly { get; internal set; }
        public bool Success
        {
            get
            {
                return CompiledAssembly != null;
            }
        }
    }
}
