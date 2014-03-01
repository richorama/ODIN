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

        private const string PREDICATE_SUBJECT = "ps";
        private const string SUBJECT_OBJECT = "so";
        private const string OBJECT_PREDICATE = "op";
        private readonly string SEPARATOR = "" + (char)254;

        public async Task Put(Triple triple)
        {
            var tasks = new List<Task>();
            tasks.Add(this.Store.Put(JoinKey(PREDICATE_SUBJECT, triple.Predicate, triple.Subject, triple.Object), triple));
            tasks.Add(this.Store.Put(JoinKey(SUBJECT_OBJECT, triple.Subject, triple.Object, triple.Predicate), triple));
            tasks.Add(this.Store.Put(JoinKey(OBJECT_PREDICATE, triple.Object, triple.Predicate, triple.Subject), triple));
            await Task.WhenAll(tasks);

        }

        public async Task Put(string subject, string predicate, string @object)
        {
            await this.Put(new Triple(subject, predicate, @object));
        }

        public Task Delete(Triple triple)
        {
            var tasks = new List<Task>();
            tasks.Add(this.Store.Delete(JoinKey(PREDICATE_SUBJECT, triple.Predicate, triple.Subject, triple.Object)));
            tasks.Add(this.Store.Delete(JoinKey(SUBJECT_OBJECT, triple.Subject, triple.Object, triple.Predicate)));
            tasks.Add(this.Store.Delete(JoinKey(OBJECT_PREDICATE, triple.Object, triple.Predicate, triple.Subject)));
            return Task.WhenAll(tasks);
        }

        public void Delete(string subject, string predicate, string @object)
        {
            this.Delete(new Triple(subject, predicate, @object));
        }

        private string JoinKey(string dimension, string value1, string value2, string value3)
        {
            return string.Join(SEPARATOR, dimension, KeyEncoder(value1), KeyEncoder(value2), KeyEncoder(value3));
        }

        public Task<IEnumerable<Triple>> Get(string subject = "", string predicate = "", string @object = "")
        {
            var hasSubject = !string.IsNullOrWhiteSpace(subject);
            var hasPredicate = !string.IsNullOrWhiteSpace(predicate);
            var hasObject = !string.IsNullOrWhiteSpace(@object);

            // this is where we wish .NET has pattern matching
            if (hasSubject && hasPredicate && hasObject)
            {
                // simple case, retrieve the entity
                return RetrieveSingleTriple(subject, predicate, @object);
            }
            if (hasSubject)
            {
                // find by subject
                if (hasPredicate)
                {
                    // subject and property
                    return QueryTriples(PREDICATE_SUBJECT, predicate, subject);
                }
                if (hasObject)
                {
                    // subject and value
                    return QueryTriples(SUBJECT_OBJECT, subject, @object);
                }
                return QueryTriples(SUBJECT_OBJECT, subject);
            }
            if (hasObject)
            {
                if (hasPredicate)
                {
                    return QueryTriples(OBJECT_PREDICATE, @object, predicate);
                }
                return QueryTriples(OBJECT_PREDICATE, @object);
            }
            if (hasPredicate)
            {
                return QueryTriples(PREDICATE_SUBJECT, predicate);
            }

            // return all triples, not recommended!
            return QueryTriples();
        }

        private async Task<IEnumerable<Triple>> RetrieveSingleTriple(string subject, string predicate, string @object)
        {
            var val = await this.Store.Get(JoinKey(SUBJECT_OBJECT, subject, @object, predicate));
            if (null != val)
            {
                return new Triple[] { new Triple(subject, predicate, @object) };
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
            return (await this.Store.Search(SUBJECT_OBJECT + SEPARATOR, SUBJECT_OBJECT + SEPARATOR + SEPARATOR)).Select(x => x.Value);
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
