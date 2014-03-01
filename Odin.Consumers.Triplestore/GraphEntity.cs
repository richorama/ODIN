
namespace Odin.Consumers.Triplestore
{
    class GraphEntity
    {

        public GraphEntity()
        {
        }

        public GraphEntity(Triple triple)
        {
            this.Subject = triple.Subject;
            this.Property = triple.Property;
            this.Value = triple.Value;
        }

        public string Subject { get; set; }
        public string Property { get; set; }
        public string Value { get; set; }

        public Triple ToTriple()
        {
            return new Triple(this.Subject, this.Property, this.Value);
        }

    }
}
