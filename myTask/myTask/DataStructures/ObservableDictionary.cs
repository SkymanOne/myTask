using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using myTask.Annotations;
using Xamarin.Forms;
using Xamarin.Forms.Internals;


namespace myTask.DataStructures
{
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private int _count;
        private bool _isReadOnly;
        private IList<TKey> _keys;
        private IList<TValue> _values;


        public ObservableDictionary()
        {
            _keys = new List<TKey>();
            _values = new List<TValue>();
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _keys = dictionary.Keys.ToList();
            _values = dictionary.Values.ToList();
        }

        public ObservableDictionary(ICollection<KeyValuePair<TKey, TValue>> collection)
        {
            foreach (KeyValuePair<TKey,TValue> pair in collection)
            {
                _keys.Add(pair.Key);
                _values.Add(pair.Value);
            }

            _count = _keys.Count;
        }
        
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new ObservableDictionaryEnum<TKey, TValue>(_keys.ToArray(), _values.ToArray());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _keys.Add(item.Key);
            _values.Add(item.Value);
            CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add,
                item.Key));
            CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add,
                item.Value));
            OnPropertyChanged(nameof(Keys));
            OnPropertyChanged(nameof(Values));
            OnPropertyChanged(nameof(Count));
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new System.NotImplementedException();
        }

        public int Count => _count;

        public bool IsReadOnly => _isReadOnly;

        public void Add(TKey key, TValue value)
        {
            _keys.Add(key);
            _values.Add(value);
            CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add,
                key));
            CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add,
                value));
           OnPropertyChanged(nameof(Keys));
           OnPropertyChanged(nameof(Values));
           OnPropertyChanged(nameof(Count));
        }

        public bool ContainsKey(TKey key)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(TKey key)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            throw new System.NotImplementedException();
        }

        public TValue this[TKey key]
        {
            get => _values.ElementAt(Keys.IndexOf(key));
            set
            {
                var oldValue = this[key];
                _values[Keys.IndexOf(key)] = value;
                CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Replace,
                    oldValue, value));
                OnPropertyChanged(nameof(Values));
            }
        }

        public ICollection<TKey> Keys => _keys;

        public ICollection<TValue> Values => _values;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ObservableDictionaryEnum<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private KeyValuePair<TKey, TValue>[] _arrayOfPair;
        private KeyValuePair<TKey, TValue> _current;
        private int _currentPosition = -1;

        public ObservableDictionaryEnum(TKey[] keys, TValue[] values)
        {
            _arrayOfPair = new KeyValuePair<TKey, TValue>[keys.Length];
            for (int i = 0; i < _arrayOfPair.Length; i++)
            {
                _arrayOfPair[i] = new KeyValuePair<TKey, TValue>(keys[i], values[i]);
            }
        }

        public bool MoveNext()
        {
            if (_currentPosition != _arrayOfPair.Length - 1)
            {
                _currentPosition++;
                _current = _arrayOfPair[_currentPosition];
                return true;
            }
            return false;
        }

        public void Reset()
        {
            _currentPosition = -1;
        }

        public KeyValuePair<TKey, TValue> Current => _current;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            //no need to implement
        }
    }
}