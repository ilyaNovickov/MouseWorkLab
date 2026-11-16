using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvaloniaDesktop.Models.Settings
{
    internal interface ICopyable<in T>
    {
        void CopyFrom(T source);
    }
}
