namespace Domain
{

    public class TestModel
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public string ParentName { get; set; }

        public string ParentCode { get; set; }
    }
}
