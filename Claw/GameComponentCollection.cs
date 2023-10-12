using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Claw.Utils;

namespace Claw
{
    /// <summary>
    /// Uma coleção de instâncias de <see cref="IGameComponent"/>.
    /// </summary>
    public sealed class GameComponentCollection : Collection<IGameComponent>
    {
        /// <summary>
        /// Filtragem dos game objects dessa coleção (fora de ordem).
        /// </summary>
        public ReadOnlyCollection<GameObject> GameObjects;
        private Collection<GameObject> gameObjects;
        public event EventHandler<IGameComponent> ComponentAdded, ComponentRemoved;

        internal GameComponentCollection()
        {
            gameObjects = new Collection<GameObject>();
            GameObjects = new ReadOnlyCollection<GameObject>(gameObjects);
        }

        /// <summary>
        /// Cria um <see cref="ComponentSortingFilteringCollection{IUpdateable}"/> configurado.
        /// </summary>
        public ComponentSortingFilteringCollection<IUpdateable> CreateForUpdate()
        {
            return new ComponentSortingFilteringCollection<IUpdateable>(this,
                u => u.Enabled,
                (u1, u2) => Comparer<int>.Default.Compare(u1.UpdateOrder, u2.UpdateOrder),
                (u, handler) => u.EnabledChanged += handler,
                (u, handler) => u.EnabledChanged -= handler,
                (u, handler) => u.UpdateOrderChanged += handler,
                (u, handler) => u.UpdateOrderChanged -= handler
            );
        }
        /// <summary>
        /// Cria um <see cref="ComponentSortingFilteringCollection{IDrawable}"/> configurado.
        /// </summary>
        public ComponentSortingFilteringCollection<IDrawable> CreateForDraw()
        {
            return new ComponentSortingFilteringCollection<IDrawable>(this,
                d => d.Visible,
                (d1, d2) => Comparer<int>.Default.Compare(d1.DrawOrder, d2.DrawOrder),
                (d, handler) => d.VisibleChanged += handler,
                (d, handler) => d.VisibleChanged -= handler,
                (d, handler) => d.DrawOrderChanged += handler,
                (d, handler) => d.DrawOrderChanged -= handler
            );
        }

        /// <summary>
        /// Adiciona um <see cref="IGameComponent"/> na coleção e chama o evento <see cref="ComponentAdded"/>.
        /// </summary>
        protected override void InsertItem(int index, IGameComponent component)
        {
            if (IndexOf(component) != -1) throw new ArgumentException("Não é permitido adicionar o mesmo componente duas vezes!");

            base.InsertItem(index, component);

            if (component != null)
            {
                OnComponentAdded(component);

                if (component is GameObject gameObject) gameObjects.Add(gameObject);

                component.Initialize();
            }
        }
        /// <summary>
        /// Remove um <see cref="IGameComponent"/> da coleção e chama o evento <see cref="ComponentRemoved"/>.
        /// </summary>
        protected override void RemoveItem(int index)
        {
            IGameComponent component = this[index];

            base.RemoveItem(index);

            if (component != null)
            {
                OnComponentRemoved(component);

                if (component is GameObject gameObject) gameObjects.Remove(gameObject);
            }
        }
        /// <summary>
        /// Remove todos os os <see cref="IGameComponent"/> da coleção e chama o evento <see cref="ComponentRemoved"/> para cada um.
        /// </summary>
        protected override void ClearItems()
        {
            for (int i = 0; i < Count; i++) OnComponentRemoved(base[i]);

            gameObjects.Clear();
            base.ClearItems();
        }

        protected override void SetItem(int index, IGameComponent item) => throw new NotSupportedException();

        private void OnComponentAdded(IGameComponent component) => ComponentAdded?.Invoke(this, component);
        private void OnComponentRemoved(IGameComponent component) => ComponentRemoved?.Invoke(this, component);
    }
}