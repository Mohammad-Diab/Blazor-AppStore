using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStore
{
    public enum DialogResult
    {
        Undefiend = 0,
        Ok = 1,
        Cancel = 2
    }

    public enum ModalConfirmButton
    {
        Default = 0,
        Add = 1,
        Edit = 2,
        Delete = 3
    }

    public enum ModalShowAnimation
    {
        None = 0,
        BounceIn = 1,
        BounceInDown = 2
    }
}
