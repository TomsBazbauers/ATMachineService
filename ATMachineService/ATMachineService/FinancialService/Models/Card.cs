namespace ATMachineService.Service.Models
{
    public class Card
    {
        public Card(string id, string customer)
        {
            CardNumber = id;
            CustomerName = customer;
        }
        public Card()
        {

        }

        public string? CardNumber { get; set; }

        public string? CustomerName { get; set; }


    }
}