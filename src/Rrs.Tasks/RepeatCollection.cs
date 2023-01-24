using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rrs.Tasks;

public class RepeatList : List<(IRepeatAsync repeat, int? priority)>
{
    public RepeatList(TimeSpan ts)
    {
        Rate = ts;
    }

    public TimeSpan Rate { get; }
    
    public void Add(IRepeatAsync r, int? priority = null) => Add((r, priority));
    public void Add(IRepeat r, int? priority = null) => Add((new RepeatedActionWrapper(r), priority));
    public void Add(Action a, int? priority = null) => Add(new RepeatedAction(Rate, a), priority);
    public void Add(Func<Task> f, int? priority = null) => Add((new RepeatedActionAsync(Rate, f), priority));
}
