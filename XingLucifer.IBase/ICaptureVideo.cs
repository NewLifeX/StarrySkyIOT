using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace XingLucifer.IBase
{
    public interface ICaptureVideo
    {
        Task<string> Open();
        void ReadStream(Action<byte[]> action, CancellationTokenSource tokenSource);
    }
}
