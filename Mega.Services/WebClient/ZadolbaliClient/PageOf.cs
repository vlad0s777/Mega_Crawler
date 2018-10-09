namespace Mega.WebClient.ZadolbaliClient
{
    using System.Collections;
    using System.Collections.Generic;

    public class PageOf<T> : IEnumerable<T>
    {
        private readonly List<T> items;

        public string Id { get; }

        public PageOf(string id)
        {
            this.items = new List<T>();
            this.Id = id;
        }

        public void Add(T item) => this.items.Add(item);

        public int Count => this.items.Count;

        public T this[int i]
        {
            get => this.items[i];
            set => this.items[i] = value;
        }

        public IEnumerator<T> GetEnumerator() => this.items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}