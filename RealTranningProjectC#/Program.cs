using RealTranningProjectC_.Abstraction;
using RealTranningProjectC_.Enums;
using RealTranningProjectC_.Interfaces;
using RealTranningProjectC_.Models;

namespace RealTranningProjectC_
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, How are you doing!? ");
            Console.WriteLine("Choose Which  way do you wanna use?\n 1-: 'cash' \n 2-: 'card' \n 3-: 'installment' \n What the number do you want? ");

            int order = int.Parse(Console.ReadLine());

            static Payment Createpayment(PaymentTypes types, double amount, string cardNumber = "", int installments = 0)
            {
                var model = new PaymentCreation
                {
                    Type = types,
                    Amount = amount,
                    CardNumber = cardNumber,
                    Installments = installments
                };
                return PaymentFactory.Create(model);
            }


            // kont momken ast5dem List of tuples or config   bdal list of payments 3shan a5ly el code aktr flexible w a5fef 3la nafsi

            List<Payment> payments = new List<Payment>();

            switch (order)
            {
                case 1:
                    payments.Add(Createpayment(Enums.PaymentTypes.Cash, 100));
                    break;

                case 2:
                    payments.Add(Createpayment(Enums.PaymentTypes.Card, 200.3543, cardNumber: "1234-5678-9012-3456"));
                    break;

                case 3:
                    payments.Add(Createpayment(Enums.PaymentTypes.Installment, 300, cardNumber: "1234-5678-9012-3456", installments: 6));
                    break;

                default:
                    Console.WriteLine("Enter correct number");
                    break;
            }

            foreach (var p in payments)
            {
                bool isValid = true;

                if (p is IValidatable v)
                {
                    isValid = v.Validate();
                }

                if (isValid)
                {
                    p.Process();
                }
                else
                {
                    Console.WriteLine("Skipped invalid payment.");
                }
            }

            Console.WriteLine("\nRefunding where possible:");

            foreach (Payment p in payments)
            {
                if (p is IRefundable r)
                {
                    r.Refund();
                }
            }
        }
    }
}