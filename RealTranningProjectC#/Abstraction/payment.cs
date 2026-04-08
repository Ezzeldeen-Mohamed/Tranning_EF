using RealTranningProjectC_.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace RealTranningProjectC_.Abstraction
{
    public abstract class Payment
    {
        public double Amount { get; }
        public DateTime PaymentDate { get; }

        protected Payment(double amount)
        {
            Amount = amount;
            PaymentDate = DateTime.Now; // add datetime .now
        }

        public abstract void Process();
    }


}
