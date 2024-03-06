using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Claw.Utils;

namespace Claw
{
    /// <summary>
    /// Uma coleção de instâncias de <see cref="IGameComponent"/>.
    /// </summary>
    public sealed class ModuleCollection : Collection<IGameModule>
    {
		/// <summary>
		/// Filtragem dos game objects desta coleção (fora de ordem).
		/// </summary>
		public ReadOnlyCollection<GameObject> GameObjects;
		public event EventHandler<IGameModule> ModuleAdded, ModuleRemoved;
		private Collection<GameObject> _gameObjects;

        internal ModuleCollection()
        {
            _gameObjects = new Collection<GameObject>();
            GameObjects = new ReadOnlyCollection<GameObject>(_gameObjects);
        }

        /// <summary>
        /// Cria um <see cref="SortedModules{IUpdateable}"/> configurado.
        /// </summary>
        public SortedModules<IUpdateable> CreateForUpdate()
        {
            return new SortedModules<IUpdateable>(this,
                u => u.Enabled,
                (u1, u2) => Comparer<int>.Default.Compare(u1.UpdateOrder, u2.UpdateOrder),
                (u, handler) => u.EnabledChanged += handler,
                (u, handler) => u.EnabledChanged -= handler,
                (u, handler) => u.UpdateOrderChanged += handler,
                (u, handler) => u.UpdateOrderChanged -= handler
            );
        }
        /// <summary>
        /// Cria um <see cref="SortedModules{IDrawable}"/> configurado.
        /// </summary>
        public SortedModules<IDrawable> CreateForDraw()
        {
            return new SortedModules<IDrawable>(this,
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
        protected override void InsertItem(int index, IGameModule module)
        {
            if (IndexOf(module) != -1) throw new ArgumentException("Não é permitido adicionar o mesmo módulo duas vezes!");

            base.InsertItem(index, module);

            if (module != null)
            {
				OnModuleAdded(module);

                if (module is GameObject gameObject) _gameObjects.Add(gameObject);

				module.Initialize();
            }
        }
        /// <summary>
        /// Remove um <see cref="IGameComponent"/> da coleção e chama o evento <see cref="ComponentRemoved"/>.
        /// </summary>
        protected override void RemoveItem(int index)
        {
            IGameModule module = this[index];

            base.RemoveItem(index);

            if (module != null)
            {
				OnModuleRemoved(module);

                if (module is GameObject gameObject) _gameObjects.Remove(gameObject);
            }
        }
        /// <summary>
        /// Remove um <see cref="IGameComponent"/> e insere outro no mesmo index.
        /// </summary>
		protected override void SetItem(int index, IGameModule newModule)
		{
			IGameModule oldModule = this[index];

			if (oldModule != null)
			{
				OnModuleRemoved(oldModule);

				if (oldModule is GameObject oldGameObject) _gameObjects.Remove(oldGameObject);

				base.SetItem(index, oldModule);

				if (oldModule is GameObject newGameObject) _gameObjects.Add(newGameObject);

				oldModule.Initialize();
			}
		}
		/// <summary>
		/// Remove todos os os <see cref="IGameComponent"/> da coleção e chama o evento <see cref="ComponentRemoved"/> para cada um.
		/// </summary>
		protected override void ClearItems()
        {
            for (int i = 0; i < Count; i++) OnModuleRemoved(base[i]);

            _gameObjects.Clear();
            base.ClearItems();
        }

        private void OnModuleAdded(IGameModule module) => ModuleAdded?.Invoke(this, module);
        private void OnModuleRemoved(IGameModule module) => ModuleRemoved?.Invoke(this, module);
    }
}