namespace Mega.Services.WebClient
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class PageOf<T> : IEnumerable<T>, IDisposable
    {
        private T[] items;

        public string Id { get; }

        public PageOf(string id)
        {
            this.items = Array.Empty<T>();
            this.Id = id;
            this.Count = 0;
        }

        public void Add(T item)
        {
            if (this.Count >= this.items.Length)
            {
                Array.Resize(ref this.items, this.items.Length + 1);
            }

            this.items[this.Count] = item;
            this.Count++;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)this.items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        public int Count { get; private set; }

        public T this[int i]
        {
            get
            {
                if (i < 0 || i >= this.Count)
                {
                    throw new IndexOutOfRangeException();
                }

                return this.items[i];
            }

            set
            {
                if (i < 0 || i >= this.Count)
                {
                    throw new IndexOutOfRangeException();
                }

                this.items[i] = value;
            }
        }

        public PageOf<T> PrevPage { get; set; }

        public void Dispose()
        {
            this.PrevPage?.Dispose();
        }
    }
}