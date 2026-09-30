using MouseBaseLib.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvalonia.Core.Interfaces
{
    public interface IModelingEnv
    {
        IModelingSettings Settings { get; }

        IMatrixRandomizer ImageRandomizer { get; }
    }
}
