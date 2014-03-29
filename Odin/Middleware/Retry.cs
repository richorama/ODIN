using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Odin.Middleware
{
    public class Retry : IOdin
    {
        public IOdin Target { get; private set; }

        public int RetryTime { get; private set; }

        public int RetryCount { get; private set; }

        public Retry(IOdin target, int retryTime = 100, int retryCount = 5)
        {
            this.Target = target;
            this.RetryTime = retryTime;
            this.RetryCount = retryCount;
        }

        private async Task RetryLogic(Func<Task> action)
        {
            var counter = 0;
            Exception ex = null;
            while (counter < this.RetryCount)
            {
                try
                {
                    await action.Invoke();
                    return;
                }
                catch (Exception exception)
                {
                    ex = exception;
                    counter += 1;
                }
                await Task.Delay(this.RetryTime);
            }
            throw ex;
        }

        public Task Put(string key, string value)
        {
            return RetryLogic(() => this.Target.Put(key, value));
        }

        public async Task<string> Get(string key)
        {
            string value = null;
            var task = new Task(async () => {
                value = await this.Target.Get(key);
            });
            await RetryLogic(() => task);
            return value;
        }


        public Task Delete(string key)
        {
            return RetryLogic(() => this.Target.Delete(key));
        }

        public async Task<IEnumerable<KeyValue>> Search(string start = null, string end = null)
        {
            IEnumerable<KeyValue> value = null;
            var task = new Task(async () =>
            {
                value = await this.Target.Search(start, end);
            });
            await RetryLogic(() => task);
            return value;

        }


    }
}
