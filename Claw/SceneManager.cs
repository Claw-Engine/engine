using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Claw
{
    /// <summary>
    /// Realiza o manuseio dos <see cref="GameObject"/>s da cena.
    /// </summary>
    public static class SceneManager
    {
        private static ReadOnlyCollection<GameObject> objects => Game.Instance.Components.GameObjects;

        /// <summary>
        /// Encontra a instância mais próxima na cena com uma tag específica.
        /// </summary>
        public static GameObject InstanceNearest(string tag, Vector2 position, bool filterEnabled = true)
        {
            IEnumerable<GameObject> objects = FindObjectsByTag(tag, filterEnabled);

            if (objects.Count() > 0) return objects.OrderBy(gameObject => Vector2.Distance(gameObject.Position, position)).First();

            return null;
        }
        /// <summary>
        /// Encontra a instância mais próxima na cena com um componente de um tipo específico.
        /// </summary>
        public static GameObject InstanceNearest<T>(Vector2 position, bool filterEnabled = true) where T : GameObject
        {
            IEnumerable<GameObject> objects = FindObjectsOfType<T>(filterEnabled);

            if (objects.Count() > 0) return objects.OrderBy(gameObject => Vector2.Distance(gameObject.Position, position)).First();

            return null;
        }

        /// <summary>
        /// Encontra um objeto com uma tag específica.
        /// </summary>
        public static GameObject FindObjectByTag(string tag, bool filterEnabled = true) => TagManager.GetObject(tag, filterEnabled);
        /// <summary>
        /// Encontra objetos com uma tag específica.
        /// </summary>
        public static IEnumerable<GameObject> FindObjectsByTag(string tag, bool filterEnabled = true) => TagManager.GetObjects(tag, filterEnabled);

        /// <summary>
        /// Retorna o número de objetos com uma tag específica.
        /// </summary>
        public static int TagCount(string tag) => TagManager.Count(tag);

        /// <summary>
        /// Encontra um objeto com um nome específico.
        /// </summary>
        public static GameObject FindObjectByName(string name, bool filterEnabled = true)
        {
            if (!filterEnabled) return objects.FirstOrDefault(gO => gO.Name == name);

            return objects.FirstOrDefault(gO => gO.Name == name && gO.Enabled);
        }
        /// <summary>
        /// Encontra objetos com um nome específico.
        /// </summary>
        public static IEnumerable<GameObject> FindObjectsByName(string name, bool filterEnabled = true)
        {
            if (!filterEnabled) return objects.Where(gO => gO.Name == name);

            return objects.Where(gO => gO.Name == name && gO.Enabled);
        }

        /// <summary>
        /// Encontra um objeto de um tipo.
        /// </summary>
        public static T FindObjectOfType<T>(bool filterEnabled = true) where T : GameObject
        {
            if (!filterEnabled) return (T)objects.FirstOrDefault(gO => gO.GetType() == typeof(T));
            
            return (T)objects.FirstOrDefault(gO => gO.GetType() == typeof(T) && gO.Enabled);
        }
        /// <summary>
        /// Encontra objetos de um tipo.
        /// </summary>
        public static IEnumerable<GameObject> FindObjectsOfType<T>(bool filterEnabled = true) where T : GameObject
        {
            var gOs = !filterEnabled ? objects.Where(gO => gO is T) : objects.Where(gO => gO is T && gO.Enabled);

            return gOs;
        }

        /// <summary>
        /// Destrói os objetos da cena.
        /// </summary>
        public static void ClearScene(bool runDestroy = false, bool deleteAll = true)
        {
            for (int i = objects.Count - 1; i >= 0; i--)
            {
                GameObject gO = objects[i];

                if (deleteAll || (!gO.DontDestroy && gO.Parent == null)) gO.SelfDestroy(runDestroy);
            }
        }

        /// <summary>
        /// Destrói um objeto.
        /// </summary>
        public static void Destroy(GameObject gameObject, bool runDestroy = true) => gameObject.SelfDestroy(runDestroy);
    }
}