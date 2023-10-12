using System;
using System.Collections.Generic;
using Claw.Extensions;

namespace Claw.Utils
{
    /// <summary>
    /// Representa um <see cref="GameComponentCollection"/> filtrado e ordenado.
    /// </summary>
    /// <typeparam name="T">Tipo que será filtrado.</typeparam>
    public sealed class ComponentSortingFilteringCollection<T>
    {
        public int Count => components.Count;

        public readonly GameComponentCollection ComponentCollection;
        private bool needSort = true;
        private Predicate<T> filter;
        private Comparison<T> sort;
        private Action<T, EventHandler<EventArgs>> filterChangedSubscriber, filterChangedUnsubscriber, sortChangedSubscriber, sortChangedUnsubscriber;
        private List<T> components;

        /// <summary>
        /// Cria uma lista de componenetes que será sempre o produto filtrado e ordenado de uma <see cref="GameComponentCollection"/>.
        /// </summary>
        /// <param name="componentCollection">A coleção original.</param>
        /// <param name="filter">A condição para a filtragem.</param>
        /// <param name="sort">O que servirá de parâmetro para a ordenação.</param>
        /// <param name="filterChangedSubscriber">Adiciona o evento que deverá ser chamado quando o filtro mudar de valor.</param>
        /// <param name="filterChangedUnsubscriber">Remove o evento que deverá ser chamado quando o filtro mudar de valor.</param>
        /// <param name="sortChangedSubscriber">Adiciona o evento que deverá ser chamado quando a ordem mudar de valor.</param>
        /// <param name="sortChangedUnsubscriber">Remove o evento que deverá ser chamado quando a ordem mudar de valor.</param>
        public ComponentSortingFilteringCollection(GameComponentCollection componentCollection, Predicate<T> filter, Comparison<T> sort, 
            Action<T, EventHandler<EventArgs>> filterChangedSubscriber, Action<T, EventHandler<EventArgs>> filterChangedUnsubscriber, 
            Action<T, EventHandler<EventArgs>> sortChangedSubscriber, Action<T, EventHandler<EventArgs>> sortChangedUnsubscriber)
        {
            if (componentCollection == null) throw new ArgumentNullException("\"componentCollection\" não pode ser nulo!");

            ComponentCollection = componentCollection;
            this.filter = filter;
            this.sort = sort;
            this.filterChangedSubscriber = filterChangedSubscriber;
            this.filterChangedUnsubscriber = filterChangedUnsubscriber;
            this.sortChangedSubscriber = sortChangedSubscriber;
            this.sortChangedUnsubscriber = sortChangedUnsubscriber;

            ComponentCollection.ComponentAdded += ComponentAdded;
            ComponentCollection.ComponentRemoved += ComponentRemoved;

            components = new List<T>();

            for (int i = 0; i < ComponentCollection.Count; i++)
            {
                if (ComponentCollection[i] is T component)
                {
                    filterChangedSubscriber(component, FilterChanged);
                    sortChangedSubscriber(component, SortChanged);

                    if (filter(component)) components.Add(component);
                }
            }

            QuickSort();
        }
        ~ComponentSortingFilteringCollection()
        {
            components.Clear();

            ComponentCollection.ComponentAdded -= ComponentAdded;
            ComponentCollection.ComponentRemoved -= ComponentRemoved;

            filter = null;
            sort = null;
            filterChangedSubscriber = null;
            filterChangedUnsubscriber = null;
            sortChangedSubscriber = null;
            sortChangedUnsubscriber = null;
        }

        /// <summary>
        /// Executa uma ação para cada elemento da coleção.
        /// </summary>
        public void ForEach(Action<T> action)
        {
            QuickSort();

            for (int i = 0; i < components.Count; i++) action(components[i]);
        }

        private void ComponentAdded(object sender, IGameComponent component)
        {
            if (component is T myType)
            {
                needSort = true;

                filterChangedSubscriber(myType, FilterChanged);
                sortChangedSubscriber(myType, SortChanged);

                if (filter(myType)) components.Add(myType);
            }
            
            QuickSort();
        }
        private void ComponentRemoved(object sender, IGameComponent component)
        {
            if (component is T myType)
            {
                components.Remove(myType);
                filterChangedUnsubscriber(myType, FilterChanged);
                sortChangedUnsubscriber(myType, SortChanged);
            }
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            T component = (T)sender;
            
            if (filter(component))
            {
                needSort = true;

                components.Add(component);
            }
            else components.Remove(component);
        }
        private void SortChanged(object sender, EventArgs e) => needSort = true;

        private int Partition(int low, int high)
        {
            T pivot = components[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (sort(components[j], pivot) < 0)
                {
                    i++;

                    components.Swap(i, j);
                }
            }

            components.Swap(i + 1, high);

            return i + 1;
        }
        private int FirstPartition(int low, int high)
        {
            int pivot = (int)(high * .5f) + 1;

            components.Swap(high, pivot);

            return Partition(low, high);
        }
        private void QuickSort(int low, int high)
        {
            if (low < high)
            {
                int pivot = FirstPartition(low, high);

                QuickSort(low, pivot - 1);
                QuickSort(pivot + 1, high);
            }
        }
        private void QuickSort()
        {
            if (needSort)
            {
                QuickSort(0, components.Count - 1);

                needSort = false;
            }
        }
    }
}