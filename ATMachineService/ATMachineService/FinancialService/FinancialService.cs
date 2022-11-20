using ATMachineService.Service.Exceptions;
using ATMachineService.Service.Interfaces;
using ATMachineService.Service.Models;
using ATMachineService.Service.Validations;

namespace ATMService.Service
{
    public class FinancialService : IFinancialService
    {
        private List<Account> _registeredAccounts;
        private readonly IFinancialOperationsValidator _operationsValidator;

        public FinancialService(List<Account> registeredAccounts, IFinancialOperationsValidator opValidator)
        {
            _registeredAccounts = registeredAccounts;
            _operationsValidator = opValidator;
        }

        public List<Fee> ChargedFees { get; set; }

        public List<ATMStatusUpdate> PendingStatusUpdates { get; set; }

        public decimal CommissionRate { get; set; }

        public bool LoadMachine(ATMStatusUpdate request)
        {
            var pendingUpdate = PendingStatusUpdates
                .FirstOrDefault(update => update.SerialNumber == request.SerialNumber && update.UpdateDate == DateTime.Today);

            if (!_operationsValidator.IsValidUpdate(request, pendingUpdate))
            {
                throw new InvalidStatusUpdateException();
            }

            return PendingStatusUpdates.Remove(pendingUpdate);
        }

        public bool IsValidCard(Card inserted, int pIN)
        {
            var account = GetAccountByNumber(inserted.CardNumber);

            return account != null && account.PIN == pIN;
        }

        public Account GetAccountByNumber(string cardNumber)
        {
            return _registeredAccounts.SingleOrDefault(acc => acc.Card.CardNumber == cardNumber)!;
        }

        public decimal GetCardBalance(Card insertedCard)
        {
            return GetAccountByNumber(insertedCard.CardNumber).Balance;
        }

        public List<Fee> GetFees(Card insertedCard)
        {
            return ChargedFees
                .Where(fee => fee.CardNumber == insertedCard.CardNumber).ToList();
        }

        public bool WithdrawMoney(string cardNumber, int amount)
        {
            var account = GetAccountByNumber(cardNumber);

            return account.Balance > amount ? SettleWithdrawal(account, amount) : false;
        }

        private bool SettleWithdrawal(Account account, int amount)
        {
            decimal commissionFee = amount * CommissionRate;

            ChargedFees.Add(new Fee(account.Card.CardNumber, amount, commissionFee));
            account.Balance -= amount + commissionFee;

            return true;
        }
    }
}