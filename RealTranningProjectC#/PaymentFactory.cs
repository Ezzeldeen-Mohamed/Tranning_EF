using RealTranningProjectC_.Abstraction;
using RealTranningProjectC_.Enums;
using RealTranningProjectC_.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTranningProjectC_
{
    class PaymentFactory
    {
        public static Payment Create(PaymentCreation Creation)
        {
                 // using enum  
               switch(Creation.Type)
                {
                    case PaymentTypes.Cash:
                        return new CashPayment(Creation.Amount);
                    case PaymentTypes.Card:
                        if (string.IsNullOrEmpty(Creation.CardNumber)) throw new ArgumentException("Need card number.");
                        return new CardPayment(Creation.Amount, Creation.CardNumber);
                    case  PaymentTypes.Installment:
                        return new InstallmentPayment(Creation.Amount, Creation.Installments);
                    default:
                        throw new ArgumentException("Unknown type.");
                }
            }
    }

}
  // add methiod prametar in view model same base entity in entity framework  

