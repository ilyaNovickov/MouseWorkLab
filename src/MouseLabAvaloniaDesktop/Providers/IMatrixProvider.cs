using System;
using System.Collections.Generic;
using System.Text;
using MouseLabAvaloniaDesktop.Models;


namespace MouseLabAvaloniaDesktop.Providers
{
    public interface IMatrixProvider
    {
        IMatrix Create(int width, int height);
    }
}
