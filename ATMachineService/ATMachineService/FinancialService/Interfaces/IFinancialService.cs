using ATMachineService.Service.Models;

namespace ATMachineService.Service.Interfaces
{
    public interface IFinancialService
    {
        List<Fee> ChargedFees { get; set; }

        List<ATMStatusUpdate> PendingStatusUpdates { get; set; }

        decimal CommissionRate { get; set; }

        Account GetAccountByNumber(string cardNumber);

        bool LoadMachine(ATMStatusUpdate request);

        bool IsValidCard(Card inserted, int pIN);

        decimal GetCardBalance(Card insertedCard);

        List<Fee> GetFees(Card insertedCard);

        bool WithdrawMoney(string cardNumber, int amount);
    }
}