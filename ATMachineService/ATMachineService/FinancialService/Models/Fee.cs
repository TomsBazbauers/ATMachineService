namespace ATMachineService.Service.Models
{
    public class Fee
    {
        public Fee(string cardNumber, int amount, decimal fee)
        {
            CardNumber = cardNumber;
            WithdrawalAmount = amount;
            WithdrawalFee = fee;
            WithdrawalDate = DateTime.Today;
        }

        public string CardNumber { get; set; }

        public int WithdrawalAmount { get; set; }

        public decimal WithdrawalFee { get; set; }

        public DateTime WithdrawalDate { get; set; }
    }
}