using System;
using System.Collections.Generic;
using Claw.Extensions;
using Claw.Modules;

namespace Claw.Utils
{
    /// <summary>
    /// Representa um <see cref="Claw.ModuleCollection"/> filtrado e ordenado.
    /// </summary>
    /// <typeparam name="T">Tipo que será filtrado.</typeparam>
    public sealed class SortedModules<TFilter>
    {
        public int Count => modules.Count;
        public readonly ModuleCollection ModuleCollection;

        private bool needSort = true;
        private Predicate<TFilter> filter;
        private Comparison<TFilter> sort;
        private Action<TFilter, Action<BaseModule>> filterChangedSubscriber, filterChangedUnsubscriber;
        private Action <TFilter, Action<TFilter>> sortChangedSubscriber, sortChangedUnsubscriber;
        private List<TFilter> modules;

        /// <summary>
        /// Cria uma lista de componenetes que será sempre o produto filtrado e ordenado de uma <see cref="Claw.ModuleCollection"/>.
        /// </summary>
        /// <param name="componentCollection">A coleção original.</param>
        /// <param name="filter">A condição para a filtragem.</param>
        /// <param name="sort">O que servirá de parâmetro para a ordenação.</param>
        /// <param name="filterChangedSubscriber">Adiciona o evento que deverá ser chamado quando o filtro mudar de valor.</param>
        /// <param name="filterChangedUnsubscriber">Remove o evento que deverá ser chamado quando o filtro mudar de valor.</param>
        /// <param name="sortChangedSubscriber">Adiciona o evento que deverá ser chamado quando a ordem mudar de valor.</param>
        /// <param name="sortChangedUnsubscriber">Remove o evento que deverá ser chamado quando a ordem mudar de valor.</param>
        public SortedModules(ModuleCollection moduleCollection, Predicate<TFilter> filter, Comparison<TFilter> sort, 
            Action<TFilter, Action<BaseModule>> filterChangedSubscriber, Action<TFilter, Action<BaseModule>> filterChangedUnsubscriber, 
            Action<TFilter, Action<TFilter>> sortChangedSubscriber, Action<TFilter, Action<TFilter>> sortChangedUnsubscriber)
        {
            if (moduleCollection == null) throw new ArgumentNullException("\"componentCollection\" não pode ser nulo!");

			ModuleCollection = moduleCollection;
            this.filter = filter;
            this.sort = sort;
            this.filterChangedSubscriber = filterChangedSubscriber;
            this.filterChangedUnsubscriber = filterChangedUnsubscriber;
            this.sortChangedSubscriber = sortChangedSubscriber;
            this.sortChangedUnsubscriber = sortChangedUnsubscriber;

			ModuleCollection.ModuleAdded += ModuleAdded;
			ModuleCollection.ModuleRemoved += ModuleRemoved;

            modules = new List<TFilter>();

            for (int i = 0; i < ModuleCollection.Count; i++)
            {
                if (ModuleCollection[i] is TFilter module)
                {
                    filterChangedSubscriber(module, FilterChanged);
                    sortChangedSubscriber(module, SortChanged);

                    if (filter(module)) modules.Add(module);
                }
            }

            QuickSort();
        }
        ~SortedModules()
        {
            modules.Clear();

            ModuleCollection.ModuleAdded -= ModuleAdded;
			ModuleCollection.ModuleRemoved -= ModuleRemoved;

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
        public void ForEach(Action<TFilter> action)
        {
            QuickSort();

            for (int i = 0; i < modules.Count; i++) action(modules[i]);
        }

        private void ModuleAdded(object sender, BaseModule module)
        {
            if (module is TFilter myType)
            {
                needSort = true;

                filterChangedSubscriber(myType, FilterChanged);
                sortChangedSubscriber(myType, SortChanged);

                if (filter(myType)) modules.Add(myType);
            }
            
            QuickSort();
        }
        private void ModuleRemoved(object sender, BaseModule module)
        {
            if (module is TFilter myType)
            {
				modules.Remove(myType);
                filterChangedUnsubscriber(myType, FilterChanged);
                sortChangedUnsubscriber(myType, SortChanged);
            }
        }

        private void FilterChanged(object sender)
        {
            TFilter module = (TFilter)sender;
            
            if (filter(module))
            {
                needSort = true;

                modules.Add(module);
            }
            else modules.Remove(module);
        }
        private void SortChanged(TFilter sender) => needSort = true;

        private int Partition(int low, int high)
        {
            TFilter pivot = modules[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (sort(modules[j], pivot) < 0)
                {
                    i++;

					modules.Swap(i, j);
                }
            }

			modules.Swap(i + 1, high);

            return i + 1;
        }
        private int FirstPartition(int low, int high)
        {
            int pivot = (int)(high * .5f) + 1;

            modules.Swap(high, pivot);

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
                QuickSort(0, modules.Count - 1);

                needSort = false;
            }
        }
    }
}