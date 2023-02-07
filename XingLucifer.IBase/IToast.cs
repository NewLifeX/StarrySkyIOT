using System;
using XingLucifer.IBase.Enums;

namespace XingLucifer.IBase
{
    public interface IToast
    {
        void Show(string str, LogLevelEnums level);
    }
}
