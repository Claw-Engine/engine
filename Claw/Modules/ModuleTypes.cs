using System;
using Claw.Graphics;

namespace Claw.Modules
{
    /// <summary>
    /// Interface para módulos que fazem parte do Step.
    /// </summary>
    public interface IStep
    {
        int StepOrder { get; }
		bool Enabled { get; }

		event Action<IStep> StepOrderChanged;
		event Action<BaseModule> EnabledChanged;

		void Step();
    }

    /// <summary>
    /// Interface para módulos que fazem parte do Render.
    /// </summary>
    public interface IRender
    {
        int RenderOrder { get; }
		bool Enabled { get; }

		event Action<IRender> RenderOrderChanged;
		event Action<BaseModule> EnabledChanged;

		void Render();
    }
}