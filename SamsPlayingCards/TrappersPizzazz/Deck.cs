using System;
using System.Collections.Generic;
using System.Text;
using NLog;



namespace TrappersPizzazz
{
    public interface IDeck : IList<ICard>
    {

        /**
         * 
         */
        void Shuffle();

        /**
         * 
         */
        void Shuffle(IShuffleStyle shuffleStyle);

        /**
         *
         */
        void Shuffle(ICollection<IShuffleStyle> shuffleStyles);

        /**
         * 
         */
        void Sort();
    }


    public interface ICard : IComparable<ICard>
    {
        Suit Suit { get; }
        FaceValue FaceValue { get; }
    }


    public enum Suit
    {
        CLUBS,
        SPADES,
        DIAMONDS,
        HEARTS
    }


    public enum FaceValue
    {
        ACE,
        KING,
        QUEEN,
        KNAVE,
        TEN,
        NINE,
        EIGHT,
        SEVEN,
        SIX,
        FIVE,
        FOUR,
        THREE,
        TWO
    }



    public class Deck : List<ICard>, IDeck
    {


        public static int MAX_CARD_COUNT = Enum.GetNames(typeof(Suit)).Length * Enum.GetNames(typeof(FaceValue)).Length;


        public Deck()
        {
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (FaceValue val in Enum.GetValues(typeof(FaceValue)))
                {
                    this.Add(new Card(suit, val));
                }
            }
        }


        public override string ToString()
        {
            StringBuilder jsonbldr = new StringBuilder("[");
            for (int i = 0; i < this.Count; i++)
            {
                jsonbldr.Append(this[i]);
                bool lastCard = i == this.Count - 1;
                if (!lastCard) jsonbldr.Append(",");
            }
            jsonbldr.Append("]");
            return jsonbldr.ToString();
        }


        public void Shuffle()
        {
            this.Shuffle(ShuffleStyle.DEFAULT);
        }

        /**
         * Write a method, function, or procedure that randomly shuffles a standard deck of 52 playing cards.
         * 
         * 
         */
        public void Shuffle(IShuffleStyle shuffleStyle)
        {
            shuffleStyle.Machine(this);
        }


        public void Shuffle(ICollection<IShuffleStyle> shuffleStyles)
        {
            foreach(IShuffleStyle shuffleStyle in shuffleStyles)
            {
                shuffleStyle.Machine(this);
            }
        }


        public override bool Equals(object obj)
        {
            bool isEqual = false;
            if (obj is IDeck)
            {
                IDeck otherDeck = (IDeck)obj;
                if (this.Count == otherDeck.Count)
                {
                    for (int i = 0; i < otherDeck.Count; i++)
                    {
                        if (!this[i].Equals(otherDeck[i])) return false; // Exit immediately
                    }
                    isEqual = true; // Card values match for each index so they are equal.
                }
            }
            return isEqual;
        }


        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

    }

    /**
     * https://en.wikipedia.org/wiki/Playing_cards_in_Unicode
     * 
     * 
     * 
     * 
     */
    public class Card : ICard
    {

        public Suit Suit { get; protected set; }
        public FaceValue FaceValue { get; protected set; }

        public Card(Suit suit, FaceValue cardval)
        {
            this.Suit = suit;
            this.FaceValue = cardval;
        }

        public int CompareTo(ICard obj)
        {
            int suitPosition = ((IComparable)Suit).CompareTo(obj.Suit);
            if (suitPosition != 0)
            {
                return suitPosition;
            } else // They are of the same suit so compare by card value
            {
                int cardValuePosition = ((IComparable)FaceValue).CompareTo(obj.FaceValue);
                return cardValuePosition;
            }
        }

        public override string ToString()
        {
            return "" + this.FaceValue  + "_OF_" + this.Suit;
        }

        public override bool Equals(object obj)
        {
            bool isEqual = false;
            if (obj is ICard)
            {
                ICard other = (ICard)obj;
                if (other.Suit == this.Suit && other.FaceValue == this.FaceValue)
                {
                    isEqual = true;
                }
            }
            return isEqual;
        }

        public override int GetHashCode()
        {
            int suitInt = (int)this.Suit + 1 * 100;
            int faceInt = (int)this.FaceValue + 1;
            int hashCode = suitInt + faceInt;
            return base.GetHashCode();
        }
    }


    public interface IShuffleStyle
    {
        void Machine(IList<ICard> deck);
    }


    // 
    //     http://www.pokerology.com/articles/how-to-shuffle-cards
    //
    public class ShuffleStyle
    {

        public static IShuffleStyle OVERHEAD = new OverheadShuffleStyle();
        // public static IShuffleStyle HINDU = new HinduShuffleStyle();
        // public static IShuffleStyle WEAVE = new WeaveShuffleStyle();
        // public static IShuffleStyle RIFFLE = new RiffleShuffleStyle();
        // public static IShuffleStyle TABLE_RIFFLE = new TableRiffleShuffleStyle();
        // public static IShuffleStyle STRIP = new StripShuffleStyle();
        // public static IShuffleStyle PORTLAND_POWERHOUSE = new PortlandPowerhouseShuffleStyle();
        // public static IShuffleStyle SHIFTWISE_FREESTYLE_SHOW_OFF_SHUFFLE = new ShiftWiseFreeStyleShowOffShuffleStyle();
        public static IShuffleStyle DEFAULT = OVERHEAD; // You get the one most people use.
    }


    public class HumanShuffleStyleHelper
    {
        public static double STANDARD_DECK_SPLIT_PRECISION = 0.2;
        public static double DEALER_FIRED_DECK_SPLIT_PRECISION = 0.7;
        public bool DEALER_IS_RIGHT_HANDED = true;
        public Random random = new Random((int)DateTime.Now.Ticks);
        public double splitPrecision = STANDARD_DECK_SPLIT_PRECISION; // Percentage of the middle the deck will be split at 0.0 is absolute precision


        public int SplitTheDeck(int cardCount)
        {
            int splitIndex = 0;

            if (splitPrecision > 0.0 && cardCount > 5)
            {
                int maxValue = (int)Math.Floor(cardCount * splitPrecision); // Number of cards the deck can be split at with given precision
                int randomNum = random.Next(maxValue);
                splitIndex = (int) Math.Floor(cardCount / 2.0 - maxValue / 2.0 + randomNum);
            }
            return splitIndex;
        }
    }


    // The Overhand Shuffle – This is the shuffle used by most people.  A good simple, lazy, sloppy shuffle.
    public class OverheadShuffleStyle : HumanShuffleStyleHelper, IShuffleStyle
    {


        public void Machine(IList<ICard> deck)
        {

            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Trace("Starting the Overhead Shuffle work.");

            int deckSplitIndex = SplitTheDeck(deck.Count);
            logger.Debug("Deck Split Index: " + deckSplitIndex);
            ICard[] deckCopy = new ICard[deck.Count];
            deck.CopyTo(deckCopy, 0);
            int left = 0;
            int right = deckSplitIndex;
            bool rightHandTurn = DEALER_IS_RIGHT_HANDED;
            bool oneHandFinished = false;
            bool lastHandWasZero = false;

            // Randomly chooses a lower and upper limit for the number of cards
            // that each hand will allow to slip when letting it's partial stack slip during the shuffle
            int lowerRandomLimit = random.Next(0, 3);
            logger.Debug("lower random limit: " + lowerRandomLimit);
            int diffBetweenLimits = random.Next(5, 9);
            logger.Debug("diff random limit: " + diffBetweenLimits);
            int upperRandomLimit = random.Next(lowerRandomLimit, lowerRandomLimit + diffBetweenLimits);
            if (upperRandomLimit <= lowerRandomLimit)
            {
                upperRandomLimit = lowerRandomLimit + 3;
            }
            logger.Debug("upper random limit: " + upperRandomLimit);

            // Main loop alternating between left and right hands letting the cards slip each loop.
            int i = 0;  // Used to assign each value to the next index
            while(i < deckCopy.Length)
            {
                
                // Zero cards is acceptable as long as it doesn't happen in multiple iterations.
                int tempLowerLimit = lowerRandomLimit;
                if (lastHandWasZero)
                {
                    tempLowerLimit = 1;  // Prevent a zero from happening twice in a row.
                }
                int qtyForHand = random.Next(tempLowerLimit, upperRandomLimit);  // The number of cards this hand will allow to slip
                if (qtyForHand == 0)
                {
                    lastHandWasZero = true;
                }

                // Allow this hands card slide for the number of cards we received from the random generator.
                logger.Debug("qtyForHand: " + qtyForHand);
                if (rightHandTurn)
                {
                    for (int handLoop = 0; handLoop < qtyForHand; handLoop++)
                    {
                        logger.Debug("Assigning Right: " + right + " to " + i);
                        deck[i++] = deckCopy[right++];
                        if (i >= deckCopy.Length || right >= deckCopy.Length) break;
                    }
                    
                } else // Left hand turn
                {
                    for (int handLoop = 0; handLoop < qtyForHand; handLoop++)
                    {
                        logger.Debug("Assigning Left: " + left + " to " + i);
                        deck[i++] = deckCopy[left++];
                        if (i >= deckCopy.Length || left >= deckSplitIndex) break;
                    }
                }

                // Check if left hand is out of cards to use
                bool leftDone = left >= deckSplitIndex;
                if (leftDone && !rightHandTurn)
                {
                    rightHandTurn = true;
                    oneHandFinished = true;
                }

                // Check if right hand is out of cards to use
                bool rightDone = right >= deckCopy.Length;
                if (rightDone && rightHandTurn)
                {
                    rightHandTurn = false;
                    oneHandFinished = true;
                }

                if (!oneHandFinished)  // Toggle hands only if one hand hasn't already finished otherwise keep it the same
                {
                    rightHandTurn = !rightHandTurn;  // Alternate
                }
               
            }
        }
    }


    // The Hindu Shuffle – A simple, quick and very elegant shuffle.One of my personal favourites.
    public class HinduShuffleStyle : IShuffleStyle
    {
        public void Machine(IList<ICard> deck)
        {
            throw new NotImplementedException();
        }
    }


    // The Weave Shuffle – A very simple shuffle to perform and for those yet to master the riffle shuffle.
    public class WeaveShuffleStyle : IShuffleStyle
    {
        public void Machine(IList<ICard> deck)
        {
            throw new NotImplementedException();
        }
    }


    // The Riffle Shuffle – This is a great way to shuffle cards and not as difficult as it looks.
    public class RiffleShuffleStyle : IShuffleStyle
    {
        public void Machine(IList<ICard> deck)
        {
            throw new NotImplementedException();
        }
    }


    // The Table Riffle Shuffle – This is easier than the in the hands riffle shuffle, yet just as effective and elegant.
    public class TableRiffleShuffleStyle : IShuffleStyle
    {
        public void Machine(IList<ICard> deck)
        {
            throw new NotImplementedException();
        }
    }


    // The Strip Shuffle – Also known as running cuts and is a great finish for the table riffle shuffle.
    public class StripShuffleStyle : IShuffleStyle
    {
        public void Machine(IList<ICard> deck)
        {
            throw new NotImplementedException();
        }
    }

    //
    public class PortlandPowerhouseShuffleStyle : IShuffleStyle
    {
        public void Machine(IList<ICard> deck)
        {
            throw new NotImplementedException();
        }
    }


    //
    public class ShiftWiseFreeStyleShowOffShuffleStyle : IShuffleStyle
    {
        public void Machine(IList<ICard> deck)
        {
            throw new NotImplementedException();
        }
    }

    

}
