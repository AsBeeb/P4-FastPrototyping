using System.Collections.Generic;
using System.Linq;

namespace CodeGeneration.Functions
{
    public class PlayLoop<T>
    {
        public PlayLoop()
        {
            List<T> Elements = new List<T>();
            List<T> TemporaryElements = new List<T>();
            T player;
            List<T> Opponents = new List<T>();
            bool Condition = false;
            // Tjekker efter alle er kørt igennem
            do
            {
                TemporaryElements = Elements;

                for (int i = 0; i < TemporaryElements.Count; i++)
                {
                    player = TemporaryElements[i];
                    Opponents = TemporaryElements.Where((x, j) => j != i).ToList();
                    // sæt programmørens kode ind
                }

            } while (!(Condition));
        }
    }
}
