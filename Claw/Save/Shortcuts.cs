using System;
using System.Collections.Generic;

namespace Claw.Save
{
    /// <summary>
    /// Atalho para <see cref="Dictionary{string, Keys}"/>.
    /// </summary>
    internal class Sections : Dictionary<string, Keys> { }
    /// <summary>
    /// Atalho para <see cref="Dictionary{string, object}"/>.
    /// </summary>
    internal class Keys : Dictionary<string, object> { }
}