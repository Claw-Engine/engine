using System;
using System.Collections.Generic;
using System.Linq;
using Claw.Extensions;

namespace Claw.Particles
{
    /// <summary>
    /// Lida com gradientes e valores fixos ou aleatórios usados por partículas.
    /// </summary>
    public struct ParticleValue<T> where T : struct
    {
        /// <summary>
        /// Define uma função que para executar o gradiente.
        /// <para>T1: Valor atual.</para>
        /// <para>T2: Valor destino.</para>
        /// <para>T3: Valor para o lerp.</para>
        /// <para>T4: Valor que diz se o <see cref="ParticleEmitterConfig.UseScaledTime"/> é verdadeiro.</para>
        /// <para>TResult: Resultado do cálculo.</para>
        /// </summary>
        public static Func<T, T, float, bool, T> GradientFunction;

        public T? FixedValue;
        public T[] ValueList;
        public bool IsGradient;
        public float LerpValue;
        private int gradientIndex;
        private T? currentValue;

        public ParticleValue(T? fixedValue)
        {
            FixedValue = fixedValue;
            ValueList = null;
            IsGradient = false;
            LerpValue = 0;
            gradientIndex = 0;
            currentValue = fixedValue;
        }
        public ParticleValue(bool isGradient, float lerpValue, params T[] valueList)
        {
            if (!valueList.IsEmpty())
            {
                ValueList = valueList;
                FixedValue = valueList[0];
            }
            else
            {
                ValueList = null;
                FixedValue = default(T);
            }

            IsGradient = isGradient;
            LerpValue = lerpValue;
            gradientIndex = 0;
            currentValue = FixedValue;
        }

        /// <summary>
        /// <para>Retorna um valor fixo, se !<see cref="IsGradient"/> e a <see cref="ValueList"/> não ter elementos.</para>
        /// <para>Retorna um valor aleatório, se !<see cref="IsGradient"/> e a <see cref="ValueList"/> ter elementos.</para>
        /// <para>Retorna o estágio do gradiente, se <see cref="IsGradient"/> e a <see cref="ValueList"/> ter elementos.</para>
        /// </summary>
        /// <param name="updateGradient">Define se o gradiente será calculado ou não.</param>
        public T? GetValue(ParticleEmitter emitter, bool updateGradient)
        {
            if (ValueList.IsEmpty() || emitter.random == null) currentValue = FixedValue;
            else if (IsGradient)
            {
                if (updateGradient && gradientIndex < ValueList.Length - 1 && GradientFunction != null && currentValue.HasValue)
                {
                    currentValue = GradientFunction.Invoke(currentValue.Value, ValueList[gradientIndex + 1], LerpValue, emitter.Config.UseScaledTime);

                    if (currentValue.Equals(ValueList[gradientIndex + 1])) gradientIndex++;
                }
            }
            else currentValue = emitter.random.Choose(ValueList);

            return currentValue;
        }
        /// <summary>
        /// Reseta o valor atual do gradiente.
        /// </summary>
        public void ResetValue()
        {
            gradientIndex = 0;

            if (!ValueList.IsEmpty()) currentValue = ValueList[0];
            else currentValue = FixedValue;
        }
    }
}