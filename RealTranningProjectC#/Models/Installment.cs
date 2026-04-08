using RealTranningProjectC_.Abstraction;
using RealTranningProjectC_.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTranningProjectC_.Models
{

    public class InstallmentPayment : Payment, IValidatable
    {
        public int Months { get; } // remove it and convert to month count 
        public const double tax = 5.0; // add taxt or additional fee if needed

        public InstallmentPayment(double amount, int installments) : base(amount)
        {
            Months = installments;
            amount += amount * tax / 100; // add tax 5% to total amount
        }

        public override void Process()
        {
            double monthly = Amount / Months;
            Console.WriteLine($"Total + Fees : {Amount}. Months: {Months}. Monthly: {monthly:F2}");

            // eh heya (F2) de 3ashan t3rd el monthly b 2 decimal places bs badal keda 2.3453 hy5aleha  2.35    bas msh3awza tsht8al
        }
        public bool Validate()
        {
            if (Months < 2 || Amount<0)
            {
                Console.WriteLine("Error: Need at least 2 installments.");
                return false;
            }
            return true;
        }
    }
}
