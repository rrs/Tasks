﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks;

public interface IRepeatAsync
{
    TimeSpan Rate { get; }
    Task OnRepeat(CancellationToken cancellationToken);
}
