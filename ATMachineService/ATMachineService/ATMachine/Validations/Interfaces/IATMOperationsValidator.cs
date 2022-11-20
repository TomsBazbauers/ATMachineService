using ATMachineService.Service.Models;

namespace ATMachineService.Machine.Machine.Validations
{
    public interface IATMOperationsValidator
    {
        bool IsValidCard(Card insertedCard);

        bool IsMachineBalanceSufficient(int request, int balance);
    }
}