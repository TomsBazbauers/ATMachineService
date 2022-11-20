namespace ATMachineService.Machine.Machine.Exceptions
{
    public class InsufficientAccountBalanceException : Exception
    {
        public InsufficientAccountBalanceException()
            : base($"Sorry, insufficient account balance. Deposit more funds to complete transaction.")
        {
        }
    }
}