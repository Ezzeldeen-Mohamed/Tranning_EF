using RealTranningProjectC_.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTranningProjectC_.Models
{
    public class PaymentCreation
    {
        //PaymentTypes type, double amount, string cardNumber = "", int installments = 0;
        public PaymentTypes Type { get; set; }
        public double Amount { get; set; }
        public string CardNumber { get; set; } = ""; 
        public int Installments { get; set; } = 0; 

    }
}
