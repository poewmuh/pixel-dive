using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace Utils._Project.Utils.Serialization
{
    [Serializable]
	[DeclareHorizontalGroup("G", Sizes = new[] { 160f })]
	public struct KeyValuePair<TKey, TValue>
	{
		[SerializeField, Group("G"), HideLabel]
		private TKey _key;

		[SerializeField, Group("G"), HideLabel, InlineProperty]
		private TValue _value;

		public TKey Key => _key;
		public TValue Value => _value;

		public KeyValuePair(System.Collections.Generic.KeyValuePair<TKey, TValue> pair)
		{
			_key = pair.Key;
			_value = pair.Value;
		}
	}

	[Serializable]
	public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
	{
		[SerializeField]
		[HideLabel, ListDrawerSettings(AlwaysExpanded = true)]
		private KeyValuePair<TKey, TValue>[] _pairs;

		private Dictionary<TKey, TValue> _dictionary;

		public Dictionary<TKey, TValue> Dictionary
		{
			get
			{
				if (_dictionary != null)
					return _dictionary;

				_dictionary = new Dictionary<TKey, TValue>(_pairs.Length);
				foreach (var t in _pairs)
					_dictionary.Add(t.Key, t.Value);

				return _dictionary;
			}
		}

		public TValue this[TKey id]
		{
			get => Dictionary[id];
			set => Dictionary[id] = value;
		}

		public KeyValuePair<TKey, TValue>[] GenerateSerializableArray()
		{
			KeyValuePair<TKey, TValue>[] pairs = new KeyValuePair<TKey, TValue>[Dictionary.Count];
			Dictionary<TKey, TValue>.Enumerator enumerator = Dictionary.GetEnumerator();

			int index = 0;
			while (enumerator.MoveNext())
			{
				pairs[index] = new KeyValuePair<TKey, TValue>(enumerator.Current);
				index++;
			}

			return pairs;
		}

		public int Count => Dictionary.Count;
		public void Add(TKey key, TValue value) => Dictionary.Add(key, value);
		public bool Remove(TKey key) => Dictionary.Remove(key);
		public void Clear() => Dictionary.Clear();
		public bool ContainsKey(TKey key) => Dictionary.ContainsKey(key);
		public bool ContainsValue(TValue value) => Dictionary.ContainsValue(value);
		public TValue Get(TKey key) => Dictionary[key];
		public bool TryGetValue(TKey id, out TValue o) => Dictionary.TryGetValue(id, out o);

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			_dictionary = new Dictionary<TKey, TValue>(_pairs.Length);
			foreach (var t in _pairs)
				_dictionary[t.Key] = t.Value;
		}
	}
}