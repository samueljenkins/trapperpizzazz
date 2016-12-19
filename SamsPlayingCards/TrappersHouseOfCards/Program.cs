using System;
using System.Collections.Generic;
using TrappersPizzazz;

namespace TrappersHouseOfCards
{
    class Program
    {

        /// <summary>
        /// This is an example of usage of the TrappersPizzazz library
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            IDeck deck = new Deck();
            deck.Shuffle();
            ICollection<IShuffleStyle> shuffles = new List<IShuffleStyle>();
            for (int i = 0; i < 1000; i++)
            {
                shuffles.Add(ShuffleStyle.DEFAULT);

            }
            deck.Shuffle(shuffles);
            Console.WriteLine("size: " + deck.Count);

            foreach(ICard card in deck)
            {
                Console.WriteLine(card);
            }

            ISet<ICard> cardSet = new SortedSet<ICard>(deck);
            Console.WriteLine("Set count: " + cardSet.Count);

            deck.Sort();
            foreach (ICard card in deck)
            {
                Console.WriteLine("sorted: " + card);
            }

            Console.ReadKey();
        }
    }
}
