using System;
using System.Collections.Generic;
using System.Text;

namespace MouseBaseLib.Interfaces.Services
{
    public interface IMoveFinderManager
    {
        int Resolution { get; set; }

        int PatchSize { get; set; }

        int SearchRange { get; set; }
    }
}
