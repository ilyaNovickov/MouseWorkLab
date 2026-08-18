using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvalonia.Core.Interfaces
{
    public interface IWindowLocator
    {
        Window GetRequiredWindow();
    }
}
