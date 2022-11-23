using ATMachineService.Machine.Machine.Exceptions;
using ATMachineService.Machine.Machine.Interfaces;
using ATMachineService.Machine.Machine.Validations;
using ATMachineService.Service.Exceptions;
using ATMachineService.Service.Interfaces;
using ATMachineService.Service.Models;

namespace ATMachineService.Machine.Machine
{
    public class ATMachine : IATMachine
    {
        private readonly IFinancialService _financialService;
        private readonly IATMOperationsValidator _customerValidator;
        private Money _currentMachineBalance;

        public ATMachine(string serialNumber,
            IFinancialService finService, IATMOperationsValidator validator)
        {
            SerialNumber = serialNumber;
            _financialService = finService;
            _customerValidator = validator;
        }

        public string SerialNumber { get; }

        public Card? CurrentCard { get; set; }

        public bool AreWithdrawalsRunning => _currentMachineBalance.Notes.All(note => note.Value >= 10);

        public Money CurrentMachineBalance
        {
            get => _currentMachineBalance;
            set => _currentMachineBalance = value;
        }

        public void LoadMoney(ATMStatusUpdate operatorRequest)
        {
            var result = _financialService.LoadMachine(operatorRequest);

            if (!result)
            {
                throw new InvalidStatusUpdateException();
            }

            _currentMachineBalance.Notes = operatorRequest.Deposit.Notes;
        }

        public void InsertCard(Card card, int pIN)
        {
            if (!AreWithdrawalsRunning)
            {
                ReturnCard();
                throw new WithdrawalsNotAvailableException();
            }

            if (!_customerValidator.IsValidCard(card) || !_financialService.IsValidCard(card, pIN))
            {
                ReturnCard();
                throw new InvalidCardException();
            }

            CurrentCard = card;
        }

        public void ReturnCard()
        {
            CurrentCard = new Card();
        }

        public decimal GetCardBalance()
        {
            return _financialService.GetCardBalance(CurrentCard);
        }

        public IEnumerable<Fee> RetrieveChargedFees()
        {
            return _financialService.GetFees(CurrentCard);
        }

        public Money WithdrawMoney(int amount)
        {
            if (!_customerValidator.IsMachineBalanceSufficient(amount, _currentMachineBalance.Amount))
            {
                throw new InsufficientATMBalanceException();
            }

            if (!_financialService.WithdrawMoney(CurrentCard.CardNumber, amount))
            {
                throw new InsufficientAccountBalanceException();
            }

            return RunWithdrawal(amount);
        }

        public Money RunWithdrawal(int amount)
        {
            var cash = new Money();

            if ((amount / 5) % 2 != 0)
            {
                cash.Notes[5] = 1;
                amount -= 5;
                _currentMachineBalance.Notes[5]--;
            }

            var noteCount = (amount - amount % 50) / 50;

            cash.Notes[50] = noteCount;
            amount -= noteCount * 50;
            _currentMachineBalance.Notes[50] -= noteCount;

            if (amount % 20 == 0)
            {
                noteCount = amount / 20;

                cash.Notes[20] = noteCount;
                amount -= noteCount * 20;
                _currentMachineBalance.Notes[20] -= noteCount;
            }
            else
            {
                noteCount = amount / 10;

                cash.Notes[10] = noteCount;
                amount -= noteCount * 10;
                _currentMachineBalance.Notes[10] -= noteCount;
            }

            return cash;
        }
    }
}
