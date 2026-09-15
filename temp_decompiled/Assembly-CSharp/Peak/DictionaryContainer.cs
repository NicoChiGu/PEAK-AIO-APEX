using System;
using System.Collections.Generic;
using UnityEngine;

namespace Peak
{
	// Token: 0x020003B4 RID: 948
	[Serializable]
	public abstract class DictionaryContainer<TKey, TValue> : ISerializationCallbackReceiver
	{
		// Token: 0x1700015F RID: 351
		// (get) Token: 0x06001869 RID: 6249 RVA: 0x0007C178 File Offset: 0x0007A378
		public Dictionary<TKey, TValue> Dict
		{
			get
			{
				if (this._dictionary.Count == 0 && this._keys.Count != 0)
				{
					int num = 0;
					while (num < this._keys.Count && num < this._values.Count)
					{
						this._dictionary[this._keys[num]] = this._values[num];
						num++;
					}
				}
				return this._dictionary;
			}
		}

		// Token: 0x0600186A RID: 6250 RVA: 0x0007C1EC File Offset: 0x0007A3EC
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			this._keys.Clear();
			this._values.Clear();
			foreach (KeyValuePair<TKey, TValue> keyValuePair in this._dictionary)
			{
				TKey tkey;
				TValue tvalue;
				keyValuePair.Deconstruct(ref tkey, ref tvalue);
				TKey tkey2 = tkey;
				TValue tvalue2 = tvalue;
				this._keys.Add(tkey2);
				this._values.Add(tvalue2);
			}
		}

		// Token: 0x0600186B RID: 6251 RVA: 0x0007C278 File Offset: 0x0007A478
		public void OnAfterDeserialize()
		{
		}

		// Token: 0x04001697 RID: 5783
		[SerializeField]
		[HideInInspector]
		private List<TKey> _keys = new List<TKey>();

		// Token: 0x04001698 RID: 5784
		[SerializeField]
		[HideInInspector]
		private List<TValue> _values = new List<TValue>();

		// Token: 0x04001699 RID: 5785
		private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
	}
}
