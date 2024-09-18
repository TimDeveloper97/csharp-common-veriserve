using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuyAnh
{
    class Status
    {
        public bool IsPass { get; set; }
        public string Message { get; set; }
    }
    class Error : Status
    {
        static public List<Error> Report { get; set; } = new List<Error>();
        static public event Action<List<Error>> OnErrorDetected;
        public Error(string message)
        {
            IsPass = false;
            Message = message;
            Report.Add(this);

            OnErrorDetected?.Invoke(Report);
        }
    }

    class Program
    {
        static Status MethodA(int i)
        {
            if (i == 6)
            {
                return new Error("6");
            }
            if (i == 7)
            {
                return new Error("7");
            }
            return new Status { IsPass = true };
        }
        static void Main(string[] args)
        {
            Error.OnErrorDetected += lst => { 
                foreach (var e in lst)
                {
                    Console.WriteLine(e.Message);
                }
            };

            MethodA(1);
            MethodA(6);
            MethodA(7);
            MethodA(2);
            MethodA(6);
        }
    }
}