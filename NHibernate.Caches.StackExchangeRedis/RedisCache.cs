using System;
using NHibernate.Cache;

namespace NHibernate.Caches.StackExchangeRedis
{
	public class RedisCache : CacheBase
	{
		public override object Get(object key)
		{
			throw new NotImplementedException();
		}

		public override void Put(object key, object value)
		{
			throw new NotImplementedException();
		}

		public override void Remove(object key)
		{
			throw new NotImplementedException();
		}

		public override void Clear()
		{
			throw new NotImplementedException();
		}

		public override void Destroy()
		{
			throw new NotImplementedException();
		}

		public override object Lock(object key)
		{
			throw new NotImplementedException();
		}

		public override void Unlock(object key, object lockValue)
		{
			throw new NotImplementedException();
		}

		public override long NextTimestamp()
		{
			throw new NotImplementedException();
		}

		public override int Timeout { get; }
		public override string RegionName { get; }
	}
}
