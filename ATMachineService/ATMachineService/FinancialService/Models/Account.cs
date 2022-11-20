namespace ATMachineService.Service.Models
{
    public class Account
    {
        private Card _card;

        public Account(Card card, decimal balance, int pIN)
        {
            _card = card;
            Balance = balance;
            PIN = pIN;
        }

        public Card Card => _card;

        public int PIN { get; set; }

        public decimal Balance { get; set; }
    }
}