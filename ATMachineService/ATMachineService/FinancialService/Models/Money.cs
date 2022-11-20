namespace ATMachineService.Service.Models
{
    public struct Money
    {
        public Money(Dictionary<int, int> notes)
        {
            Notes = notes;
        }

        public Money()
        {
            Notes = new Dictionary<int, int>()
            {
                { 5, 0 },
                { 10, 0 },
                { 20, 0 },
                { 50, 0 }
            };
        }

        public Dictionary<int, int> Notes { get; set; }

        public int Amount => Notes.Sum(note => note.Key * note.Value);
    }
}