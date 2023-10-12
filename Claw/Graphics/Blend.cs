namespace Claw.Graphics
{
    /// <summary>
    /// Define os tipos de blend factor.
    /// </summary>
    public enum  Blend
    {
        /// <summary>
        /// 0; 0; 0; 0.
        /// </summary>
        Zero = 0x1,
        /// <summary>
        /// 1; 1; 1; 1.
        /// </summary>
        One = 0x2,
        /// <summary>
        /// srcR; srcG; srcB; srcA.
        /// </summary>
        SourceColor = 0x3,
        /// <summary>
        /// 1 - srcR; 1 - srcG; 1 - srcB; 1 - srcA.
        /// </summary>
        OneMinusSourceColor = 0x4,
        /// <summary>
        /// srcA; srcA; srcA; srcA.
        /// </summary>
        SourceAlpha = 0x5,
        /// <summary>
        /// 1 - srcA;  1 - srcA; 1 - srcA; 1 - srcA.
        /// </summary>
        OneMinusSourceAlpha = 0x6,
        /// <summary>
        /// dstR; dstG; dstB; dstA.
        /// </summary>
        DestinationColor = 0x7,
        /// <summary>
        ///  1 - dstR; 1 - dstG; 1 - dstB; 1 - dstA.
        /// </summary>
        OneMinusDestinationColor = 0x8,
        /// <summary>
        /// dstA; dstA; dstA; dstA.
        /// </summary>
        DestinationAlpha = 0x9,
        /// <summary>
        /// 1 - dstA; 1 - dstA; 1 - dstA; 1 - dstA.
        /// </summary>
        OneMinusDestinationAlpha = 0xA
    }
}