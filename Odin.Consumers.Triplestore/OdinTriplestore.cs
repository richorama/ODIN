using Odin.JsonSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Odin.Consumers.Triplestore
{
    public class OdinTriplestore
    {
        OdinJsonSerializer<Triple> Store { get; set; }

        public OdinTriplestore(IOdin odin)
        {
            this.Store = new OdinJsonSerializer<Triple>(odin);
            this.KeyEncoder = MD5Hash;
        }

        private const string PROPERTY_SUBJECT = "ps";
        private const string SUBJECT_VALUE = "sv";
        private const string VALUE_PROPERTY = "vp";
        private readonly string SEPARATOR = "" + (char)254;

        public async Task Put(Triple triple)
        {
            var tasks = new List<Task>();
            tasks.Add(this.Store.Put(JoinKey(PROPERTY_SUBJECT, triple.Property, triple.Subject, triple.Value), triple));
            tasks.Add(this.Store.Put(JoinKey(SUBJECT_VALUE, triple.Subject, triple.Value, triple.Property), triple));
            tasks.Add(this.Store.Put(JoinKey(VALUE_PROPERTY, triple.Value, triple.Property, triple.Subject), triple));
            await Task.WhenAll(tasks);

        }

        public async Task Put(string subject, string property, string value)
        {
            await this.Put(new Triple(subject, property, value));
        }

        public Task Delete(Triple triple)
        {
            var tasks = new List<Task>();
            tasks.Add(this.Store.Delete(JoinKey(PROPERTY_SUBJECT, triple.Property, triple.Subject, triple.Value)));
            tasks.Add(this.Store.Delete(JoinKey(SUBJECT_VALUE, triple.Subject, triple.Value, triple.Property)));
            tasks.Add(this.Store.Delete(JoinKey(VALUE_PROPERTY, triple.Value, triple.Property, triple.Subject)));
            return Task.WhenAll(tasks);
        }

        public void Delete(string subject, string property, string value)
        {
            this.Delete(new Triple(subject, property, value));
        }

        private string JoinKey(string dimension, string value1, string value2, string value3)
        {
            return string.Join(SEPARATOR, dimension, KeyEncoder(value1), KeyEncoder(value2), KeyEncoder(value3));
        }

        public Task<IEnumerable<Triple>> Get(string subject = "", string property = "", string value = "")
        {
            var hasSubject = !string.IsNullOrWhiteSpace(subject);
            var hasProperty = !string.IsNullOrWhiteSpace(property);
            var hasValue = !string.IsNullOrWhiteSpace(value);

            // this is where we wish .NET has pattern matching
            if (hasSubject && hasProperty && hasValue)
            {
                // simple case, retrieve the entity
                return RetrieveSingleTriple(subject, property, value);
            }
            if (hasSubject)
            {
                // find by subject
                if (hasProperty)
                {
                    // subject and property
                    return QueryTriples(PROPERTY_SUBJECT, property, subject);
                }
                if (hasValue)
                {
                    // subject and value
                    return QueryTriples(SUBJECT_VALUE, subject, value);
                }
                return QueryTriples(SUBJECT_VALUE, subject);
            }
            if (hasValue)
            {
                if (hasProperty)
                {
                    return QueryTriples(VALUE_PROPERTY, value, property);
                }
                return QueryTriples(VALUE_PROPERTY, value);
            }
            if (hasProperty)
            {
                return QueryTriples(PROPERTY_SUBJECT, property);
            }

            // return all triples, not recommended!
            return QueryTriples();
        }

        private async Task<IEnumerable<Triple>> RetrieveSingleTriple(string subject, string property, string value)
        {
            var val = await this.Store.Get(JoinKey(SUBJECT_VALUE, subject, value, property));
            if (null != val)
            {
                return new Triple[] { new Triple(subject, property, value) };
            }
            return new Triple[] { };
        }

        private async Task<IEnumerable<Triple>> QueryTriples(string dimension, string pk1, string pk2)
        {
            var subRange = string.Join(SEPARATOR, dimension, pk1, pk2);
            return (await this.Store.Search(subRange, subRange + SEPARATOR)).Select(x => x.Value);
        }

        private async Task<IEnumerable<Triple>> QueryTriples(string dimension, string pk1)
        {
            var subRange = string.Join(SEPARATOR, dimension, KeyEncoder(pk1));
            return (await this.Store.Search(subRange + SEPARATOR, subRange + SEPARATOR + SEPARATOR)).Select(x => x.Value);
        }

        private async Task<IEnumerable<Triple>> QueryTriples()
        {
            return (await this.Store.Search(SUBJECT_VALUE + SEPARATOR, SUBJECT_VALUE + SEPARATOR + SEPARATOR)).Select(x => x.Value);
        }

        public Func<string, string> KeyEncoder { get; set; }

        public static string MD5Hash(string input)
        {
            var hash = MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(input));

            var sb = new StringBuilder();
            foreach (var b in hash)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
