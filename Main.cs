using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (MethodInfo r in typeof (Program)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .Where(m => m.Name == "GetDateSelection")
                .Select(m => m.Invoke(null, new object[] {DateTime.Today})))
            {
                Console.WriteLine(r.ToString());
            }
            //var selection = GetDateSelection(startingSelection: DateTime.Today);
            //Console.Clear();
            //Console.WriteLine(selection.ToShortDateString() + " was the selected date");
        }

        private static readonly Dictionary<ConsoleKey, Func<DateTime, DateTime>> _dateModByKey = new Dictionary<ConsoleKey, Func<DateTime, DateTime>>
        {
            {ConsoleKey.LeftArrow,  d => d.AddDays(-1)},
            {ConsoleKey.RightArrow, d => d.AddDays(1)},
            {ConsoleKey.UpArrow,    d => d.AddDays(-7)},
            {ConsoleKey.DownArrow,  d => d.AddDays(7)},
            {ConsoleKey.PageUp,     d => d.AddMonths(-1)},
            {ConsoleKey.PageDown,   d => d.AddMonths(1)},
            {ConsoleKey.Home,       d => d.AddYears(-1)},
            {ConsoleKey.End,        d => d.AddYears(1)}
        };

        private static DateTime GetDateSelection(DateTime startingSelection)
        {
            
            return InputsUntilConfirmation()
                .Where(key => _dateModByKey.ContainsKey(key))
                .Aggregate(Print(startingSelection), 
                    (current, key) => Print(_dateModByKey[key](current)));
        }

        private static DateTime Print(DateTime d)
        {
            Console.Clear();
            foreach (var s in GetBuffer(d)) Console.Write(s);
            return d;
        }

        private static IEnumerable<DateTime> DaysInMonth(DateTime d)
        {
            for (var i = GetFirstOfMonth(d); i.Month == d.Month; i = i.AddDays(1))
                yield return i.Date;
        }

        private static IEnumerable<string> GetBuffer(DateTime selection)
        {   
            return GetHeader(selection)
                .Union(DaysInMonth(selection).Select(x => 
                        (x.Day == 1 ? new string('\t', (int) GetFirstOfMonth(selection).DayOfWeek) : "") +
                        x.Day + 
                        (x == selection ? "<-" : "") + 
                        (x.DayOfWeek == DayOfWeek.Saturday ? "\n" : "\t")));
        }

        private static IEnumerable<string> GetHeader(DateTime selection)
        {
            return new[]
            {
                selection.ToString("MMMM yyyy\n"),
                "Sun\tMon\tTues\tWed\tThur\tFri\tSat\n"
            };
        }

        private static IEnumerable<ConsoleKey> InputsUntilConfirmation()
        {
            for (var k = Console.ReadKey().Key; !IsConfirm(k); k = Console.ReadKey().Key)
            {
                yield return k;
            }
        }

        private static bool IsConfirm(ConsoleKey k)
        {
            return k == ConsoleKey.Enter;
        }

        private static DateTime GetFirstOfMonth(DateTime d)
        {
            return d.AddDays(1 - d.Day);
        }
    }
}
