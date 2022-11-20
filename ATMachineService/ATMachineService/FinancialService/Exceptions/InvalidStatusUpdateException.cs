namespace ATMachineService.Service.Exceptions
{
    public class InvalidStatusUpdateException : Exception
    {
        public InvalidStatusUpdateException() 
            : base($"Status update properties do not match the required properties. Retry or exit.")
        {
        }
    }
}