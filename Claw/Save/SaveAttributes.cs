using System;

namespace Claw.Save
{
    /// <summary>
    /// Indica que a propriedade será registrada no save, mesmo que privada.
    /// </summary>
    public class SavePropertyAttribute : Attribute
    {
        public readonly string Name;

        public SavePropertyAttribute() => Name = null;
        public SavePropertyAttribute(string name) => Name = name;
    }

    /// <summary>
    /// Indica que a propriedade será ignorada do save.
    /// </summary>
    public class SaveIgnoreAttribute : Attribute { }
}