using ATMachineService.Service.Models;

namespace ATMachineService.Machine.Machine.Interfaces
{
    public interface IATMachine
    {
        string SerialNumber { get; }

        public Money CurrentMachineBalance { get; set; }

        bool AreWithdrawalsRunning { get; }

        decimal GetCardBalance();

        Card? CurrentCard { get; set; }

        void InsertCard(Card card, int pIN);

        void LoadMoney(ATMStatusUpdate operatorRequest);

        IEnumerable<Fee> RetrieveChargedFees();

        void ReturnCard();

        Money WithdrawMoney(int amount);

        Money RunWithdrawal(int amount);
    }
}