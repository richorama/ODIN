using Odin.JsonSerializer;
using Odin.Middleware;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Odin.Consumers.GeoSpatial
{
    public class OdinGeoSpatial
    {
        public OdinJsonSerializer<PositionKeyValue> GeoIndex { get; private set; }
        public IOdin KeyIndex { get; private set; }

        private static string CreateGeoKey(Position position, string key)
        {
            return string.Format("{0}{1}{2}", position.GetQuadKey(), ((char)254), key);
        }

        public OdinGeoSpatial(IOdin odin)
        {
            this.GeoIndex = new OdinJsonSerializer<PositionKeyValue>(new Partition(odin, "GI"));
            this.KeyIndex = new Partition(odin, "KI");
        }

        public async Task Put(Position position, string key, string value)
        {
            // we should think about having some kind of transaction concept around this
            var pkv = new PositionKeyValue { Key = key, Value = value, Latitude = position.Latitude, Longitude = position.Longitude };
            var quadKey = CreateGeoKey(position, key);

            var existingQuadKey = await this.KeyIndex.Get(key);
            if (null != (existingQuadKey))
            {
                // the value already exists, so delete it
                await this.GeoIndex.Delete(existingQuadKey);
            }

            await Task.WhenAll(this.GeoIndex.Put(quadKey, pkv), this.KeyIndex.Put(key, quadKey));
        }

        public async Task<PositionKeyValue> Get(string key)
        {
            var quadKey = await this.KeyIndex.Get(key);
            return await this.GeoIndex.Get(quadKey);
        }

        public async Task Delete(string key)
        {
            var quadKey = await this.KeyIndex.Get(key);
            await Task.WhenAll(this.KeyIndex.Delete(key), this.GeoIndex.Delete(quadKey));
        }

        public Task<IEnumerable<PositionKeyValueDistance>> Search(Position position, double radius)
        {
            // this is work in progress
            throw new NotImplementedException();
        }

    }
}
