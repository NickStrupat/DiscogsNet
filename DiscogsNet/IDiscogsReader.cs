using System;
using System.Collections.Generic;

namespace DiscogsNet
{
    public interface IDiscogsReader<T> : IDisposable
    {
        T Read();
        IEnumerable<T> Enumerate();
        double EstimatedProgress { get; }
    }
}
