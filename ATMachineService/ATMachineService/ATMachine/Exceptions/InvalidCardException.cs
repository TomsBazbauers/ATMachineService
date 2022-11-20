namespace ATMachineService.Machine.Machine.Exceptions
{
    public class InvalidCardException : Exception
    {
        public InvalidCardException() : base($"Invalid card. Please check your PIN and try again.")
        {
        }
    }
}