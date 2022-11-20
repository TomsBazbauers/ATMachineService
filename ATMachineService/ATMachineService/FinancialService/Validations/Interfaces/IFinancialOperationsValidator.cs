using ATMachineService.Service.Models;

namespace ATMachineService.Service.Validations
{
    public interface IFinancialOperationsValidator
    {
        bool IsValidUpdate(ATMStatusUpdate request, ATMStatusUpdate pending);

        bool IsValidDeposit(Money expected, Money deposited);

        bool IsValidPassword(Guid expected, Guid received);
    }
}
