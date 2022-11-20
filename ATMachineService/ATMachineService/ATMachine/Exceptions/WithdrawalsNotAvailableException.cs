namespace ATMachineService.Machine.Machine.Exceptions
{
    public class WithdrawalsNotAvailableException : Exception
    {
        public WithdrawalsNotAvailableException() : base($"Sorry, currently withdrawals are unavailable.")
        {
        }
    }
}