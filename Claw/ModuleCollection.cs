using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Claw.Modules;
using Claw.Utils;

namespace Claw
{
	/// <summary>
	/// Uma coleção de instâncias de <see cref="BaseModule"/>.
	/// </summary>
	public sealed class ModuleCollection : Collection<BaseModule>
    {
		public event EventHandler<BaseModule> ModuleAdded, ModuleRemoved;

        /// <summary>
        /// Cria um <see cref="SortedModules{IStep}"/> configurado.
        /// </summary>
        public SortedModules<IStep> CreateForStep()
        {
            return new SortedModules<IStep>(this,
                (s) => s.Enabled,
                (s1, s2) => Comparer<int>.Default.Compare(s1.StepOrder, s2.StepOrder),
                (s, handler) => s.EnabledChanged += handler,
                (s, handler) => s.EnabledChanged -= handler,
                (s, handler) => s.StepOrderChanged += handler,
                (s, handler) => s.StepOrderChanged -= handler
            );
        }
        /// <summary>
        /// Cria um <see cref="SortedModules{IRender}"/> configurado.
        /// </summary>
        public SortedModules<IRender> CreateForRender()
        {
            return new SortedModules<IRender>(this,
                (r) => r.Enabled,
                (r1, r2) => Comparer<int>.Default.Compare(r1.RenderOrder, r2.RenderOrder),
                (r, handler) => r.EnabledChanged += handler,
                (r, handler) => r.EnabledChanged -= handler,
                (r, handler) => r.RenderOrderChanged += handler,
                (r, handler) => r.RenderOrderChanged -= handler
            );
        }

        /// <summary>
        /// Adiciona um <see cref="IGameComponent"/> na coleção e chama o evento <see cref="ComponentAdded"/>.
        /// </summary>
        protected override void InsertItem(int index, BaseModule module)
        {
            if (IndexOf(module) != -1) throw new ArgumentException("Não é permitido adicionar o mesmo módulo duas vezes!");

            base.InsertItem(index, module);

            if (module != null)
            {
				OnModuleAdded(module);
				module.Initialize();
            }
        }
        /// <summary>
        /// Remove um <see cref="IGameComponent"/> da coleção e chama o evento <see cref="ComponentRemoved"/>.
        /// </summary>
        protected override void RemoveItem(int index)
        {
			BaseModule module = this[index];

            base.RemoveItem(index);

            if (module != null) OnModuleRemoved(module);
		}
        /// <summary>
        /// Remove um <see cref="IGameComponent"/> e insere outro no mesmo index.
        /// </summary>
		protected override void SetItem(int index, BaseModule newModule)
		{
			BaseModule oldModule = this[index];

			if (oldModule != null)
			{
				OnModuleRemoved(oldModule);
				base.SetItem(index, oldModule);
				oldModule.Initialize();
			}
		}
		/// <summary>
		/// Remove todos os os <see cref="IGameComponent"/> da coleção e chama o evento <see cref="ComponentRemoved"/> para cada um.
		/// </summary>
		protected override void ClearItems()
        {
            for (int i = 0; i < Count; i++) OnModuleRemoved(base[i]);

            base.ClearItems();
        }

        private void OnModuleAdded(BaseModule module) => ModuleAdded?.Invoke(this, module);
        private void OnModuleRemoved(BaseModule module) => ModuleRemoved?.Invoke(this, module);
    }
}