using System;
using System.Linq;

namespace DatabaseManagerCli
{
    class ProgressReporter
    {
        private DateTime lastReport = DateTime.MinValue;

        public ProgressReporter()
        {
        }

        public void Report(string label, double progress)
        {
            if (progress == -1 || DateTime.Now.Subtract(this.lastReport).TotalSeconds >= 1)
            {
                Console.WriteLine(label + ": " + Math.Abs(progress).ToString("0.00%"));
                this.lastReport = DateTime.Now;
            }
        }
    }
}
