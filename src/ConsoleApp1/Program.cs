// See https://aka.ms/new-console-template for more information
using Rrs.Tasks;

var values = new List<DateTime>();

var t = new IntervalTimer(TimeSpan.FromSeconds(1), t => values.Add(DateTime.Now));
t.Start();

await Task.Delay(TimeSpan.FromSeconds(10));

t.Dispose();

foreach(var v in values)
{
    Console.WriteLine(v.ToString("ss.ffff"));
}