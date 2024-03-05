using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Claw.Maps;
using Claw.Utils;

namespace Claw
{
    /// <summary>
    /// Uma coleção de instâncias de <see cref="IGameComponent"/>.
    /// </summary>
    public sealed class ComponentCollection : Collection<IGameComponent>
    {
		/// <summary>
		/// Filtragem dos game objects desta coleção (fora de ordem).
		/// </summary>
		public ReadOnlyCollection<GameObject> GameObjects;
        private Collection<GameObject> _gameObjects;
        public event EventHandler<IGameComponent> ComponentAdded, ComponentRemoved;

        internal ComponentCollection()
        {
            _gameObjects = new Collection<GameObject>();
            GameObjects = new ReadOnlyCollection<GameObject>(_gameObjects);
        }

        /// <summary>
        /// Cria um <see cref="SortedComponents{IUpdateable}"/> configurado.
        /// </summary>
        public SortedComponents<IUpdateable> CreateForUpdate()
        {
            return new SortedComponents<IUpdateable>(this,
                u => u.Enabled,
                (u1, u2) => Comparer<int>.Default.Compare(u1.UpdateOrder, u2.UpdateOrder),
                (u, handler) => u.EnabledChanged += handler,
                (u, handler) => u.EnabledChanged -= handler,
                (u, handler) => u.UpdateOrderChanged += handler,
                (u, handler) => u.UpdateOrderChanged -= handler
            );
        }
        /// <summary>
        /// Cria um <see cref="SortedComponents{IDrawable}"/> configurado.
        /// </summary>
        public SortedComponents<IDrawable> CreateForDraw()
        {
            return new SortedComponents<IDrawable>(this,
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

                if (component is GameObject gameObject) _gameObjects.Add(gameObject);

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

                if (component is GameObject gameObject) _gameObjects.Remove(gameObject);
            }
        }
        /// <summary>
        /// Remove um <see cref="IGameComponent"/> e insere outro no mesmo index.
        /// </summary>
		protected override void SetItem(int index, IGameComponent newComponent)
		{
			IGameComponent oldComponent = this[index];

			if (oldComponent != null)
			{
				OnComponentRemoved(oldComponent);

				if (oldComponent is GameObject oldGameObject) _gameObjects.Remove(oldGameObject);

				base.SetItem(index, newComponent);

				if (newComponent is GameObject newGameObject) _gameObjects.Add(newGameObject);

				newComponent.Initialize();
			}
		}
		/// <summary>
		/// Remove todos os os <see cref="IGameComponent"/> da coleção e chama o evento <see cref="ComponentRemoved"/> para cada um.
		/// </summary>
		protected override void ClearItems()
        {
            for (int i = 0; i < Count; i++) OnComponentRemoved(base[i]);

            _gameObjects.Clear();
            base.ClearItems();
        }

        private void OnComponentAdded(IGameComponent component) => ComponentAdded?.Invoke(this, component);
        private void OnComponentRemoved(IGameComponent component) => ComponentRemoved?.Invoke(this, component);
    }
}