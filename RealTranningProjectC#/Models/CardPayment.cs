using RealTranningProjectC_.Abstraction;
using RealTranningProjectC_.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTranningProjectC_.Models
{
    public class CardPayment : Payment, IRefundable, IValidatable
    {
        public string CardNumber { get; }

        public CardPayment(double amount, string cardNumber) : base(amount)
        {
            CardNumber = cardNumber;
        }

        public override void Process()
        {
            Console.WriteLine($"Paid {Amount} with card {CardNumber} on {PaymentDate}. Charged!");
        }

        public double Refund()
        {
            Console.WriteLine($"[Cash Refund] Refunded {Amount} to card on {PaymentDate}.");
            return Amount;
        }

        public bool Validate() // no amount with netgative or zero
        {
            if (Amount <= 0)
            {
                Console.WriteLine("Error: Card payment amount must be more than 0.");
                return false;
            }

            if (string.IsNullOrEmpty(CardNumber))
            {
                Console.WriteLine("Error: Card number can't be empty.");
                return false;
            }
            return true;
        }
    }
}
