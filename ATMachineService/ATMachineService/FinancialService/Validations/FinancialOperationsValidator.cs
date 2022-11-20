using ATMachineService.Service.Models;

namespace ATMachineService.Service.Validations
{
    public class FinancialOperationsValidator : IFinancialOperationsValidator
    {
        public bool IsValidUpdate(ATMStatusUpdate request, ATMStatusUpdate pending)
        {
            return IsValidPassword(request.Password, pending.Password)
                && IsValidDeposit(request.Deposit, pending.Deposit);
        }

        public bool IsValidPassword(Guid request, Guid pending)
        {
            return request == pending;
        }

        public bool IsValidDeposit(Money requestDeposit, Money pendingDeposit)
        {
            return pendingDeposit.Notes.OrderBy(pair => pair.Key)
                .SequenceEqual(requestDeposit.Notes.OrderBy(pair => pair.Key));
        }
    }
}