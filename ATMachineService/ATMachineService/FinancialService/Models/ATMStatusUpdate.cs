namespace ATMachineService.Service.Models
{
    public class ATMStatusUpdate
    {
        public ATMStatusUpdate(string id, Money deposit, Guid password, DateTime updateDate)
        {
            SerialNumber = id;
            Deposit = deposit;
            Password = password;
            UpdateDate = updateDate.Date;
        }

        public string SerialNumber { get; set; }

        public Money Deposit { get; set; }

        public Guid Password { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}