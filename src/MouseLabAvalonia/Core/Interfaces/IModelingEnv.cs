using MouseBaseLib.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvalonia.Core.Interfaces
{
    public interface IModelingEnv
    {
        public IMatrixRandomizer ImageRandomizer { get; }
    }
}
