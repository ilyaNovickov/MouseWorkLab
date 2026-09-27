using System;
using System.Collections.Generic;
using System.Text;
using MouseLab.Core.Models;

namespace MouseLab.Core.Providers
{
    public interface IMatrixProvider
    {
        IMatrix Create(int width, int height);
    }
}
