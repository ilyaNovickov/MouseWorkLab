using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvaloniaDesktop.Models
{
    public interface IDataTransferObject<T>
    {
        T ToDto();

        void LoadDto(T value);
    }
}
