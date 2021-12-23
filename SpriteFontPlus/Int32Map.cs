using System;
using System.Collections;
using System.Collections.Generic;

namespace SpriteFontPlus;

public sealed class Int32Map<TValue> : IEnumerable<KeyValuePair<int, TValue>> {
    int[]? _buckets;
    Entry[]? _entries;
    int _count;
    int _version;
    int _freeList;
    int _freeCount;

    public Int32Map() : this(0) { }

    public Int32Map(int capacity) {
        if (capacity < 0) {
            ThrowHelper.ArgumentOutOfRangeException();
        }
        if (capacity > 0) {
            Initialize(capacity);
        }
    }

    public int Count {
        get { return _count - _freeCount; }
    }

    public TValue this[int key] {
        get {
            unchecked {
                if (_buckets != null) {
                    var buckets = _buckets!;
                    var entries = _entries!;
                    var num = key & int.MaxValue;
                    for (var index = buckets[num % buckets.Length]; index >= 0; index = entries[index].Next) {
                        if (entries[index].Key == key) {
                            return entries[index].Value;
                        }
                    }
                }
                ThrowHelper.KeyNotFoundException();
                return default!;
            }
        }
        set { Insert(key, value, false); }
    }

    IEnumerator<KeyValuePair<int, TValue>> IEnumerable<KeyValuePair<int, TValue>>.GetEnumerator() {
        return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        throw new NotImplementedException();
    }

    public void Add(int key, TValue value) {
        Insert(key, value, true);
    }

    public void Clear() {
        if (_count <= 0) {
            return;
        }
        var buckets = _buckets!;
        var entries = _entries!;
        for (var index = 0; index < buckets.Length; ++index) {
            buckets[index] = -1;
        }
        Array.Clear(entries, 0, _count);
        _freeList = -1;
        _count = 0;
        _freeCount = 0;
        _version++;
    }

    public bool ContainsKey(int key) {
        return FindEntry(key) >= 0;
    }

    int FindEntry(int key) {
        unchecked {
            if (_buckets != null) {
                var buckets = _buckets!;
                var entries = _entries!;
                var num = key & int.MaxValue;
                for (var index = buckets[num % buckets.Length]; index >= 0; index = entries[index].Next) {
                    if (entries[index].Key == key) {
                        return index;
                    }
                }
            }
            return -1;
        }
    }

    void Initialize(int capacity) {
        var prime = SizingHelper.GetSizingPrime(capacity);
        _buckets = new int[prime];
        for (var index = 0; index < _buckets.Length; ++index) {
            _buckets[index] = -1;
        }
        _entries = new Entry[prime];
        _freeList = -1;
    }

    void Insert(int key, TValue value, bool add) {
        unchecked {
            if (_buckets == null) {
                Initialize(0);
            }
            var buckets = _buckets!;
            var entries = _entries!;
            var num1 = key & int.MaxValue;
            var index1 = num1 % buckets.Length;
            var num2 = 0;
            for (var index2 = buckets[index1]; index2 >= 0; index2 = entries[index2].Next) {
                if (entries[index2].Key == key) {
                    if (add) {
                        ThrowHelper.ArgumentException();
                    }
                    entries[index2].Value = value;
                    _version++;
                    return;
                }
                ++num2;
            }
            int index3;
            if (_freeCount > 0) {
                index3 = _freeList;
                _freeList = entries[index3].Next;
                _freeCount--;
            }
            else {
                if (_count == entries.Length) {
                    Resize();
                    buckets = _buckets!;
                    entries = _entries!;
                    index1 = num1 % buckets.Length;
                }
                index3 = _count;
                _count++;
            }
            entries[index3].HashCode = num1;
            entries[index3].Next = buckets[index1];
            entries[index3].Key = key;
            entries[index3].Value = value;
            buckets[index1] = index3;
            _version++;
            if (num2 <= 100) {
                return;
            }
            Resize(entries.Length + (entries.Length / 4) + 1);
        }
    }

    void Resize() {
        Resize(SizingHelper.NextSizingPrime(_count));
    }

    void Resize(int newSize) {
        var numArray = new int[newSize];
        for (var index = 0; index < numArray.Length; ++index) {
            numArray[index] = -1;
        }
        var entryArray = new Entry[newSize];
        if (_count > 0) {
            Array.Copy(_entries!, 0, entryArray!, 0, _count);
        }
        for (var index1 = 0; index1 < _count; ++index1) {
            if (entryArray[index1].HashCode >= 0) {
                var index2 = entryArray[index1].HashCode % newSize;
                entryArray[index1].Next = numArray[index2];
                numArray[index2] = index1;
            }
        }
        _buckets = numArray;
        _entries = entryArray;
    }

    public bool Remove(int key) {
        unchecked {
            if (_buckets != null) {
                var buckets = _buckets!;
                var entries = _entries!;
                var num = key & int.MaxValue;
                var index1 = num % buckets.Length;
                var index2 = -1;
                for (var index3 = buckets[index1]; index3 >= 0; index3 = entries[index3].Next) {
                    if (entries[index3].Key == key) {
                        if (index2 < 0) {
                            buckets[index1] = entries[index3].Next;
                        }
                        else {
                            entries[index2].Next = entries[index3].Next;
                        }
                        entries[index3].HashCode = -1;
                        entries[index3].Next = _freeList;
                        entries[index3].Key = default;
                        entries[index3].Value = default!;
                        _freeList = index3;
                        _freeCount++;
                        _version++;
                        return true;
                    }
                    index2 = index3;
                }
            }
            return false;
        }
    }

    public bool TryGetValue(int key, out TValue value) {
        unchecked {
            if (_buckets != null) {
                var buckets = _buckets!;
                var entries = _entries!;
                var num = key & int.MaxValue;
                for (var index = buckets[num % buckets.Length]; index >= 0; index = entries[index].Next) {
                    if (entries[index].Key == key) {
                        value = entries[index].Value;
                        return true;
                    }
                }
            }
            value = default!;
            return false;
        }
    }

    public Enumerator GetEnumerator() {
        return new(this);
    }

    struct Entry {
        public int HashCode;
        public int Next;
        public int Key;
        public TValue Value;
    }

    public struct Enumerator : IEnumerator<KeyValuePair<int, TValue>> {
        readonly Int32Map<TValue> _parent;
        readonly int _version;
        int _index;

        public void Reset() {
            throw new NotImplementedException();
        }

        object IEnumerator.Current {
            get { return Current; }
        }

        public KeyValuePair<int, TValue> Current { get; set; }

        internal Enumerator(Int32Map<TValue> parent) {
            _parent = parent;
            _version = parent._version;
            _index = 0;
            Current = new();
        }

        public bool MoveNext() {
            if (_version != _parent._version) {
                ThrowHelper.InvalidOperationException();
            }
            var entries = _parent._entries!;
            for (; (uint)_index < (uint)_parent._count; _index++) {
                if (entries[_index].HashCode >= 0) {
                    Current = new(entries[_index].Key, entries[_index].Value);
                    _index++;
                    return true;
                }
            }
            _index = _parent._count + 1;
            Current = new();
            return false;
        }

        public void Dispose() { }
    }
}
