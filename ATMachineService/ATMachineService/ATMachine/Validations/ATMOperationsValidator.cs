using ATMachineService.Service.Models;

namespace ATMachineService.Machine.Machine.Validations
{
    public class ATMOperationsValidator : IATMOperationsValidator
    {
        public bool IsValidCard(Card insertedCard)
        {
            return !string.IsNullOrEmpty(insertedCard.CardNumber.Trim())
                && !string.IsNullOrEmpty(insertedCard.CustomerName.Trim());
        }

        public bool IsMachineBalanceSufficient(int request, int balance)
        {
            return request < balance - request;
        }
    }
}
