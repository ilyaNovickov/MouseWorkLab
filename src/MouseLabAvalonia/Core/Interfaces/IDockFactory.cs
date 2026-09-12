using Dock.Model.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvalonia.Core.Interfaces
{
    public interface IDockFactory : IFactory
    {
        void AddDockable(string vmName);
    }
}
