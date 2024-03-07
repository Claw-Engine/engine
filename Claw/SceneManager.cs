using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Claw.Modules;

namespace Claw
{
    /// <summary>
    /// Realiza o manuseio da cena.
    /// </summary>
    public static class SceneManager
    {
        /// <summary>
        /// Encontra a instância mais próxima na cena com uma tag específica.
        /// </summary>
        public static BaseModule InstanceNearest(string tag, Vector2 position, bool filterEnabled = true)
        {
            IEnumerable<BaseModule> modules = FindModulesByTag(tag, filterEnabled);

            if (modules.Count() > 0) return modules.OrderBy(module => Vector2.Distance(module.Transform.Position, position)).First();

            return null;
        }
        /// <summary>
        /// Encontra a instância mais próxima de um tipo específico.
        /// </summary>
        public static BaseModule InstanceNearest<T>(Vector2 position, bool filterEnabled = true) where T : BaseModule
        {
            IEnumerable<BaseModule> modules = FindModulesOfType<T>(filterEnabled);

            if (modules.Count() > 0) return modules.OrderBy(module => Vector2.Distance(module.Transform.Position, position)).First();

            return null;
        }

        /// <summary>
        /// Encontra um módulo com uma tag específica.
        /// </summary>
        public static BaseModule FindModuleByTag(string tag, bool filterEnabled = true) => TagManager.GetModule(tag, filterEnabled);
        /// <summary>
        /// Encontra módulos com uma tag específica.
        /// </summary>
        public static IEnumerable<BaseModule> FindModulesByTag(string tag, bool filterEnabled = true) => TagManager.GetModules(tag, filterEnabled);

        /// <summary>
        /// Retorna o número de módulos com uma tag específica.
        /// </summary>
        public static int TagCount(string tag) => TagManager.Count(tag);

        /// <summary>
        /// Encontra um módulo com um nome específico.
        /// </summary>
        public static BaseModule FindModuleByName(string name, bool filterEnabled = true)
        {
            if (!filterEnabled) return Game.Instance.Modules.FirstOrDefault(gO => gO.Name == name);

            return Game.Instance.Modules.FirstOrDefault(gO => gO.Name == name && gO.Enabled);
        }
        /// <summary>
        /// Encontra módulos com um nome específico.
        /// </summary>
        public static IEnumerable<BaseModule> FindModulesByName(string name, bool filterEnabled = true)
        {
            if (!filterEnabled) return Game.Instance.Modules.Where(gO => gO.Name == name);

            return Game.Instance.Modules.Where(gO => gO.Name == name && gO.Enabled);
        }

        /// <summary>
        /// Encontra um módulo de um tipo.
        /// </summary>
        public static T FindModuleOfType<T>(bool filterEnabled = true) where T : BaseModule
        {
            if (!filterEnabled) return (T)Game.Instance.Modules.FirstOrDefault(gO => gO.GetType() == typeof(T));
            
            return (T)Game.Instance.Modules.FirstOrDefault(gO => gO.GetType() == typeof(T) && gO.Enabled);
        }
        /// <summary>
        /// Encontra módulos de um tipo.
        /// </summary>
        public static IEnumerable<BaseModule> FindModulesOfType<T>(bool filterEnabled = true) where T : BaseModule
        {
            var gOs = !filterEnabled ? Game.Instance.Modules.Where(gO => gO is T) : Game.Instance.Modules.Where(gO => gO is T && gO.Enabled);

            return gOs;
        }

        /// <summary>
        /// Destrói os módulos da cena.
        /// </summary>
        public static void ClearScene(bool runDestroy = false, bool deleteAll = true)
        {
            BaseModule current;

			for (int i = Game.Instance.Modules.Count - 1; i >= 0; i--)
            {
                current = Game.Instance.Modules[i];
                
                if (deleteAll || (!current.DontDestroy && current.Transform.Parent == null)) current.SelfDestroy(runDestroy);
            }
        }

        /// <summary>
        /// Destrói um módulo.
        /// </summary>
        public static void Destroy(BaseModule module, bool runDestroy = true) => module.SelfDestroy(runDestroy);
    }
}