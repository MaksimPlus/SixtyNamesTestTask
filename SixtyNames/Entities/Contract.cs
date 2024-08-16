namespace SixtyNames.Entities
{
    public class Contract
    {
        public int Id { get; set; }
        public double ContractSum { get; set; }
        public string Status { get; set; }
        public DateTime DateOfSigning { get; set; }
        public PhysicalPerson PhysicalPerson { get; set; }
        public LegalEntity LegalEntity { get; set; }
    }
}
