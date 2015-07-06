using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Assets.Scripts.Utilities.Collections
{

    public delegate void NotifyCollectionChangedEventHandler(ICollection source, NotifyCollectionChangedEventArgs e);

    public interface INotifyCollectionChanged
    {
        event NotifyCollectionChangedEventHandler CollectionChanged;
    }

    [Serializable]
    public class ObservableCollection<T> : Collection<T>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public new void Clear()
        {
            IList oldItems = this;

            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, oldItems));
        }

        public new void Add(T item)
        {
            base.Add(item);
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public new void Remove(T item)
        {
            base.Remove(item);
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);

            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public new void RemoveAt(int index)
        {
            object oldItem = null;

            oldItem = this[index];
            base.RemoveAt(index);
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem));
        }

        public void Concat(IEnumerable<T> second)
        {
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add);

            foreach (T newItem in second)
            {
                base.Add(newItem);
                args.NewItems.Add(newItem);
            }

            if (CollectionChanged != null)
                CollectionChanged(this, args);
        }

        public new T this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                T oldValue = base[index];

                base[index] = value;
                if (CollectionChanged != null)
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue, index));
            }
        }
    }

    public class ObservableDictionary<TKey, TValue> :
    IDictionary<TKey, TValue>,
    ICollection<KeyValuePair<TKey, TValue>>,
    IEnumerable<KeyValuePair<TKey, TValue>>,
    IDictionary,
    ICollection,
    IEnumerable,
    ISerializable,
    IDeserializationCallback,
    INotifyPropertyChanged,
    INotifyCollectionChanged,
    IXmlSerializable
    {
        #region constructors

        #region public

        public ObservableDictionary()
        {
            _keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>();
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>();

            foreach (KeyValuePair<TKey, TValue> entry in dictionary)
                DoAddEntry((TKey)entry.Key, (TValue)entry.Value);
        }

        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            _keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>(comparer);
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            _keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>(comparer);

            foreach (KeyValuePair<TKey, TValue> entry in dictionary)
                DoAddEntry((TKey)entry.Key, (TValue)entry.Value);
        }

        #endregion public

        #region protected

        protected ObservableDictionary(SerializationInfo info, StreamingContext context)
        {
            _siInfo = info;
        }

        #endregion protected

        #endregion constructors

        #region properties

        #region public

        public IEqualityComparer<TKey> Comparer
        {
            get { return _keyedEntryCollection.Comparer; }
        }

        public int Count
        {
            get { return _keyedEntryCollection.Count; }
        }

        public Dictionary<TKey, TValue>.KeyCollection Keys
        {
            get { return TrueDictionary.Keys; }
        }

        public TValue this[TKey key]
        {
            get { return (TValue)_keyedEntryCollection[key].Value; }
            set { DoSetEntry(key, value); }
        }

        public T GetValueSafely<T>(TKey key, T defaultValue)
        {
            TValue val;
            if (this.TryGetValue(key, out val))
                return (T)((object)val);

            return defaultValue;
        }

        public Dictionary<TKey, TValue>.ValueCollection Values
        {
            get { return TrueDictionary.Values; }
        }

        #endregion public

        #region private

        private Dictionary<TKey, TValue> TrueDictionary
        {
            get
            {
                if (_dictionaryCacheVersion != _version)
                {
                    _dictionaryCache.Clear();
                    foreach (DictionaryEntry entry in _keyedEntryCollection)
                        _dictionaryCache.Add((TKey)entry.Key, (TValue)entry.Value);
                    _dictionaryCacheVersion = _version;
                }
                return _dictionaryCache;
            }
        }

        #endregion private

        #endregion properties

        #region methods

        #region public

        public void Add(TKey key, TValue value)
        {
            DoAddEntry(key, value);
        }

        public void Clear()
        {
            DoClearEntries();
        }

        public bool ContainsKey(TKey key)
        {
            return _keyedEntryCollection.Contains(key);
        }

        public bool ContainsValue(TValue value)
        {
            return TrueDictionary.ContainsValue(value);
        }

        public IEnumerator GetEnumerator()
        {
            return new Enumerator<TKey, TValue>(this, false);
        }

        public bool Remove(TKey key)
        {
            return DoRemoveEntry(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            bool result = _keyedEntryCollection.Contains(key);
            value = result ? (TValue)_keyedEntryCollection[key].Value : default(TValue);
            return result;
        }

        #endregion public

        #region protected

        protected virtual bool AddEntry(TKey key, TValue value)
        {
            _keyedEntryCollection.Add(new DictionaryEntry(key, value));
            return true;
        }

        protected virtual bool ClearEntries()
        {
            // check whether there are entries to clear
            bool result = (Count > 0);
            if (result)
            {
                // if so, clear the dictionary
                _keyedEntryCollection.Clear();
            }
            return result;
        }

        protected int GetIndexAndEntryForKey(TKey key, out DictionaryEntry entry)
        {
            entry = new DictionaryEntry();
            int index = -1;
            if (_keyedEntryCollection.Contains(key))
            {
                entry = _keyedEntryCollection[key];
                index = _keyedEntryCollection.IndexOf(entry);
            }
            return index;
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, args);
        }

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        protected virtual bool RemoveEntry(TKey key)
        {
            // remove the entry
            return _keyedEntryCollection.Remove(key);
        }

        protected virtual bool SetEntry(TKey key, TValue value)
        {
            bool keyExists = _keyedEntryCollection.Contains(key);

            // if identical key/value pair already exists, nothing to do
            if (keyExists && value.Equals((TValue)_keyedEntryCollection[key].Value))
                return false;

            // otherwise, remove the existing entry
            if (keyExists)
                _keyedEntryCollection.Remove(key);

            // add the new entry
            _keyedEntryCollection.Add(new DictionaryEntry(key, value));

            return true;
        }

        #endregion protected

        #region private

        private void DoAddEntry(TKey key, TValue value)
        {
            if (AddEntry(key, value))
            {
                _version++;

                DictionaryEntry entry;
                int index = GetIndexAndEntryForKey(key, out entry);
                FireEntryAddedNotifications(entry, index);

                if (value is INotifyPropertyChanged)
                {
                    ((INotifyPropertyChanged)value).PropertyChanged += DictionaryItem_PropertyChanged;
                }
            }
        }

        void DictionaryItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (CollectionItemPropertyChanged != null)
                CollectionItemPropertyChanged(sender, e);
        }

        private void DoClearEntries()
        {
            if (ClearEntries())
            {
                _version++;
                FireResetNotifications();
            }
        }

        private bool DoRemoveEntry(TKey key)
        {
            DictionaryEntry entry;
            int index = GetIndexAndEntryForKey(key, out entry);

            bool result = RemoveEntry(key);
            if (result)
            {
                _version++;
                if (index > -1)
                {
                    FireEntryRemovedNotifications(entry, index);
                    if (entry.Value is INotifyPropertyChanged)
                        ((INotifyPropertyChanged)entry.Value).PropertyChanged -= DictionaryItem_PropertyChanged;
                }
            }

            return result;
        }

        private void DoSetEntry(TKey key, TValue value)
        {
            DictionaryEntry entry;
            int index = GetIndexAndEntryForKey(key, out entry);

            if (SetEntry(key, value))
            {
                _version++;

                // if prior entry existed for this key, fire the removed notifications
                if (index > -1)
                {
                    FireEntryRemovedNotifications(entry, index);

                    // force the property change notifications to fire for the modified entry
                    _countCache--;
                }

                // then fire the added notifications
                index = GetIndexAndEntryForKey(key, out entry);
                FireEntryAddedNotifications(entry, index);
            }
        }

        private void FireEntryAddedNotifications(DictionaryEntry entry, int index)
        {
            // fire the relevant PropertyChanged notifications
            FirePropertyChangedNotifications();

            // fire CollectionChanged notification
            if (index > -1)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>((TKey)entry.Key, (TValue)entry.Value), index));
            else
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void FireEntryRemovedNotifications(DictionaryEntry entry, int index)
        {
            // fire the relevant PropertyChanged notifications
            FirePropertyChangedNotifications();

            // fire CollectionChanged notification
            if (index > -1)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>((TKey)entry.Key, (TValue)entry.Value), index));
            else
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void FirePropertyChangedNotifications()
        {
            if (Count != _countCache)
            {
                _countCache = Count;
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                OnPropertyChanged("Keys");
                OnPropertyChanged("Values");
            }
        }

        private void FireResetNotifications()
        {
            // fire the relevant PropertyChanged notifications
            FirePropertyChangedNotifications();

            // fire CollectionChanged notification
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion private

        #endregion methods

        #region interfaces

        #region IDictionary<TKey, TValue>

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            DoAddEntry(key, value);
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            return DoRemoveEntry(key);
        }

        bool IDictionary<TKey, TValue>.ContainsKey(TKey key)
        {
            return _keyedEntryCollection.Contains(key);
        }

        bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            return TryGetValue(key, out value);
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get { return Keys; }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get { return Values; }
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get { return (TValue)_keyedEntryCollection[key].Value; }
            set { DoSetEntry(key, value); }
        }

        #endregion IDictionary<TKey, TValue>

        #region IDictionary

        void IDictionary.Add(object key, object value)
        {
            DoAddEntry((TKey)key, (TValue)value);
        }

        void IDictionary.Clear()
        {
            DoClearEntries();
        }

        bool IDictionary.Contains(object key)
        {
            return _keyedEntryCollection.Contains((TKey)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new Enumerator<TKey, TValue>(this, true);
        }

        bool IDictionary.IsFixedSize
        {
            get { return false; }
        }

        bool IDictionary.IsReadOnly
        {
            get { return false; }
        }

        object IDictionary.this[object key]
        {
            get { return _keyedEntryCollection[(TKey)key].Value; }
            set
            {
                DoSetEntry((TKey)key, (TValue)value);

            }
        }

        ICollection IDictionary.Keys
        {
            get { return Keys; }
        }

        void IDictionary.Remove(object key)
        {
            DoRemoveEntry((TKey)key);
        }

        ICollection IDictionary.Values
        {
            get { return Values; }
        }

        #endregion IDictionary

        #region ICollection<KeyValuePair<TKey, TValue>>

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> kvp)
        {
            DoAddEntry(kvp.Key, kvp.Value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            DoClearEntries();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> kvp)
        {
            return _keyedEntryCollection.Contains(kvp.Key);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("CopyTo() failed:  array parameter was null");
            }
            if ((index < 0) || (index > array.Length))
            {
                throw new ArgumentOutOfRangeException("CopyTo() failed:  index parameter was outside the bounds of the supplied array");
            }
            if ((array.Length - index) < _keyedEntryCollection.Count)
            {
                throw new ArgumentException("CopyTo() failed:  supplied array was too small");
            }

            foreach (DictionaryEntry entry in _keyedEntryCollection)
                array[index++] = new KeyValuePair<TKey, TValue>((TKey)entry.Key, (TValue)entry.Value);
        }

        int ICollection<KeyValuePair<TKey, TValue>>.Count
        {
            get { return _keyedEntryCollection.Count; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> kvp)
        {
            return DoRemoveEntry(kvp.Key);
        }

        #endregion ICollection<KeyValuePair<TKey, TValue>>

        #region ICollection

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_keyedEntryCollection).CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return _keyedEntryCollection.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)_keyedEntryCollection).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)_keyedEntryCollection).SyncRoot; }
        }

        #endregion ICollection

        #region IEnumerable<KeyValuePair<TKey, TValue>>

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return new Enumerator<TKey, TValue>(this, false);
        }

        #endregion IEnumerable<KeyValuePair<TKey, TValue>>

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable

        #region ISerializable

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            Collection<DictionaryEntry> entries = new Collection<DictionaryEntry>();
            foreach (DictionaryEntry entry in _keyedEntryCollection)
                entries.Add(entry);
            info.AddValue("entries", entries);
        }

        #endregion ISerializable

        #region IDeserializationCallback

        public virtual void OnDeserialization(object sender)
        {
            if (_siInfo != null)
            {
                Collection<DictionaryEntry> entries = (Collection<DictionaryEntry>)
                    _siInfo.GetValue("entries", typeof(Collection<DictionaryEntry>));
                foreach (DictionaryEntry entry in entries)
                    AddEntry((TKey)entry.Key, (TValue)entry.Value);
            }
        }

        #endregion IDeserializationCallback

        #region INotifyCollectionChanged

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add { CollectionChanged += value; }
            remove { CollectionChanged -= value; }
        }

        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion INotifyCollectionChanged

        #region INotifyPropertyChanged

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
        }

        protected virtual event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangedEventHandler CollectionItemPropertyChanged;

        #endregion INotifyPropertyChanged

        #endregion interfaces

        #region protected classes

        #region KeyedDictionaryEntryCollection<TKey>

        protected class KeyedDictionaryEntryCollection<TKey> : KeyedCollection<TKey, DictionaryEntry>
        {
            #region constructors

            #region public

            public KeyedDictionaryEntryCollection() : base() { }

            public KeyedDictionaryEntryCollection(IEqualityComparer<TKey> comparer) : base(comparer) { }

            #endregion public

            #endregion constructors

            #region methods

            #region protected

            protected override TKey GetKeyForItem(DictionaryEntry entry)
            {
                return (TKey)entry.Key;
            }

            #endregion protected

            #endregion methods
        }

        #endregion KeyedDictionaryEntryCollection<TKey>

        #endregion protected classes

        #region public structures

        #region Enumerator

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Enumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IDictionaryEnumerator, IEnumerator
        {
            #region constructors

            internal Enumerator(ObservableDictionary<TKey, TValue> dictionary, bool isDictionaryEntryEnumerator)
            {
                _dictionary = dictionary;
                _version = dictionary._version;
                _index = -1;
                _isDictionaryEntryEnumerator = isDictionaryEntryEnumerator;
                _current = new KeyValuePair<TKey, TValue>();
            }

            #endregion constructors

            #region properties

            #region public

            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    ValidateCurrent();
                    return _current;
                }
            }

            #endregion public

            #endregion properties

            #region methods

            #region public

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                ValidateVersion();
                _index++;
                if (_index < _dictionary._keyedEntryCollection.Count)
                {
                    _current = new KeyValuePair<TKey, TValue>((TKey)_dictionary._keyedEntryCollection[_index].Key, (TValue)_dictionary._keyedEntryCollection[_index].Value);
                    return true;
                }
                _index = -2;
                _current = new KeyValuePair<TKey, TValue>();
                return false;
            }

            #endregion public

            #region private

            private void ValidateCurrent()
            {
                if (_index == -1)
                {
                    throw new InvalidOperationException("The enumerator has not been started.");
                }
                else if (_index == -2)
                {
                    throw new InvalidOperationException("The enumerator has reached the end of the collection.");
                }
            }

            private void ValidateVersion()
            {
                if (_version != _dictionary._version)
                {
                    throw new InvalidOperationException("The enumerator is not valid because the dictionary changed.");
                }
            }

            #endregion private

            #endregion methods

            #region IEnumerator implementation

            object IEnumerator.Current
            {
                get
                {
                    ValidateCurrent();
                    if (_isDictionaryEntryEnumerator)
                    {
                        return new DictionaryEntry(_current.Key, _current.Value);
                    }
                    return new KeyValuePair<TKey, TValue>(_current.Key, _current.Value);
                }
            }

            void IEnumerator.Reset()
            {
                ValidateVersion();
                _index = -1;
                _current = new KeyValuePair<TKey, TValue>();
            }

            #endregion IEnumerator implemenation

            #region IDictionaryEnumerator implemenation

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    ValidateCurrent();
                    return new DictionaryEntry(_current.Key, _current.Value);
                }
            }
            object IDictionaryEnumerator.Key
            {
                get
                {
                    ValidateCurrent();
                    return _current.Key;
                }
            }
            object IDictionaryEnumerator.Value
            {
                get
                {
                    ValidateCurrent();
                    return _current.Value;
                }
            }

            #endregion

            #region fields

            private ObservableDictionary<TKey, TValue> _dictionary;
            private int _version;
            private int _index;
            private KeyValuePair<TKey, TValue> _current;
            private bool _isDictionaryEntryEnumerator;

            #endregion fields
        }

        #endregion Enumerator

        #endregion public structures

        #region fields

        protected KeyedDictionaryEntryCollection<TKey> _keyedEntryCollection;

        private int _countCache = 0;
        private Dictionary<TKey, TValue> _dictionaryCache = new Dictionary<TKey, TValue>();
        private int _dictionaryCacheVersion = 0;
        private int _version = 0;

        [NonSerialized]
        private SerializationInfo _siInfo = null;

        #endregion fields

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");

                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                this.Add(key, value);

                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");

                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();

                writer.WriteStartElement("value");
                //TODO check for basic types (string, int, etc) if TValue is object.
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }
    }

    public class NotifyCollectionChangedEventArgs : EventArgs
    {
        private NotifyCollectionChangedAction _Action;
        public NotifyCollectionChangedAction Action
        {
            get
            {
                return _Action;
            }

            private set
            {
                _Action = value;
            }
        }

        private int _OldStartingIndex = -1;
        public int OldStartingIndex
        {
            get
            {
                return _OldStartingIndex;
            }

            private set
            {
                _OldStartingIndex = value;
            }

        }

        private IList _OldItems = new List<object>();
        public IList OldItems
        {
            get
            {
                return _OldItems;
            }

            private set
            {
                _OldItems = value;
            }
        }

        int _NewStartingIndex = -1;
        public int NewStartingIndex
        {
            get
            {
                return _NewStartingIndex;
            }
            private set
            {
                _NewStartingIndex = value;
            }
        }

        private IList _NewItems = new List<object>();
        public IList NewItems
        {
            get
            {
                return _NewItems;
            }

            set
            {
                _NewItems = value;
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
        {
            Action = action;
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem)
        {
            Action = action;

            if (action == NotifyCollectionChangedAction.Add || action == NotifyCollectionChangedAction.Replace || action == NotifyCollectionChangedAction.Move)
                NewItems.Add(changedItem);

            else

                OldItems.Add(changedItem);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems)
        {
            Action = action;

            if (action == NotifyCollectionChangedAction.Add || action == NotifyCollectionChangedAction.Replace || action == NotifyCollectionChangedAction.Move)
                NewItems = changedItems;

            else
                OldItems = changedItems;
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index)
        {
            Action = action;

            if (action == NotifyCollectionChangedAction.Add || action == NotifyCollectionChangedAction.Replace || action == NotifyCollectionChangedAction.Move)
            {
                NewItems.Add(changedItem);
                NewStartingIndex = index;
            }

            else
            {
                OldItems.Add(changedItem);
                OldStartingIndex = index;
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem)
        {
            Action = action;

            NewItems.Add(newItem);
            OldItems.Add(oldItem);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int newItemIndex)
        {
            Action = action;
            NewItems.Add(newItem);
            OldItems.Add(oldItem);
            NewStartingIndex = newItemIndex;
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems)
        {
            Action = action;
            NewItems = newItems;
            OldItems = oldItems;
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex)
        {
            Action = action;
            NewItems = newItems;
            OldItems = oldItems;
            NewStartingIndex = startingIndex;
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex, int oldIndex)
        {
            Action = action;
            NewItems = newItems;
            OldItems = oldItems;
            NewStartingIndex = startingIndex;
            OldStartingIndex = OldStartingIndex;
        }
    }

    public enum NotifyCollectionChangedAction
    {
        Add,
        Move,
        Remove,
        Replace,
        Reset
    }
}
