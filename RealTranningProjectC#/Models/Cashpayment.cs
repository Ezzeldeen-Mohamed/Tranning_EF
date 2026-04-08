using RealTranningProjectC_.Abstraction;
using RealTranningProjectC_.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTranningProjectC_.Models
{

    public class CashPayment : Payment, IValidatable, IRefundable    // add irefundable interface
    {
        public CashPayment(double amount) : base(amount) { }

        public override void Process()
        {
            Console.WriteLine($"Paid {Amount} in cash on {PaymentDate}. Done!");
        }

        public bool Validate()
        {
            if (Amount <= 0)
            {
                Console.WriteLine("Error: Cash amount must be more than 0.");
                return false;
            }
            return true;
        }
        public double Refund()
        {
            Console.WriteLine($"[Cash Refund] Returning {Amount} in cash.");
            return Amount;
        }
    }
}
