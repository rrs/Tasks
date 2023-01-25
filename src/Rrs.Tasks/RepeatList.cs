using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks;

public class RepeatList : List<(IDoAsync repeat, int? priority)>
{
    public RepeatList(TimeSpan ts)
    {
        Rate = ts;
    }

    public TimeSpan Rate { get; }
    
    public void Add(IDoAsync r, int? priority = null) => Add((r, priority));
    public void Add(IDo r, int? priority = null) => Add((new DoActionWrapper(r), priority));
    public void Add(Action<CancellationToken> a, int? priority = null) => Add(new DoAction(a), priority);
    public void Add(Func<CancellationToken, Task> f, int? priority = null) => Add((new DoActionAsync(f), priority));
}
