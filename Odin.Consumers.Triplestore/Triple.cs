using System;

namespace Odin.Consumers.Triplestore
{
    public class Triple
    {
        public Triple(string subject, string predicate, string @object)
        {
            if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentNullException("subject");
            if (string.IsNullOrWhiteSpace(predicate)) throw new ArgumentNullException("predicate");
            if (string.IsNullOrWhiteSpace(@object)) throw new ArgumentNullException("object");

            this.Subject = subject;
            this.Predicate = predicate;
            this.Object = @object;
        }

        public string Subject { get; private set; }
        public string Predicate { get; private set; }
        public string Object { get; private set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", this.Subject, this.Predicate, this.Object);
        }
    }
}
