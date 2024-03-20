using System;

namespace Clawssets
{
	internal class Program
	{
		static void Main(string[] args)
		{
			using (Main game = new Main()) game.Run();
		}
	}
}