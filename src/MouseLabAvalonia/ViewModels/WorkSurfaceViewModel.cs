using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvalonia.ViewModels
{
    public partial class WorkSurfaceViewModel : ViewModelBase
    {




#if DEBUG
        public static WorkSurfaceViewModel Instance => new WorkSurfaceViewModel();
#endif
    }
}
