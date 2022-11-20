namespace ATMachineService.Machine.Machine.Exceptions
{
    public class InsufficientATMBalanceException : Exception
    {
        public InsufficientATMBalanceException()
            : base($"Insufficient machine balance. Sorry, decrease withdrawal amount or try another ATM.")
        {
        }
    }
}