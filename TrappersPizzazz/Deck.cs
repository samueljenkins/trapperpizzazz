using System;
using System.Collections.Generic;
using System.Text;
using NLog;


/// <summary>
/// 
/// A code sample project written by Samuel Jenkins
/// 
/// This small library provides a way to get a collection of playing card values
/// and provides a means of sorting those values using the default IList Sort method and
/// provides a way of shuffling the cards using an implementation of IShuffleStyle interface
/// or by selecting the default which takes no arguments.
/// 
/// </summary>
namespace TrappersPizzazz
{
    public interface IDeck : IList<ICard>
    {


        /// <summary>
        /// Default method to shuffle the cards
        /// </summary>
        void Shuffle();


        /// <summary>
        /// Provides a way to customize the shuffle algorithm to your liking
        /// </summary>
        /// <param name="shuffleStyle"></param>
        void Shuffle(IShuffleStyle shuffleStyle);


        /// <summary>
        /// This method makes it possible to link any number of shuffle styles together
        /// </summary>
        /// <param name="shuffleStyles"></param>
        void Shuffle(ICollection<IShuffleStyle> shuffleStyles);

        /// <summary>
        /// Ensures the sort method is available
        /// </summary>
        void Sort();
    }

    /// <summary>
    /// The interface the cards must implement in order to be used by the Deck
    /// </summary>
    public interface ICard : IComparable<ICard>
    {
        Suit Suit { get; }
        FaceValue FaceValue { get; }
    }

    /// <summary>
    /// The suit for the card
    /// </summary>
    public enum Suit
    {
        CLUBS,
        SPADES,
        DIAMONDS,
        HEARTS
    }

    /// <summary>
    /// The face of the card less the suit
    /// </summary>
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


    /// <summary>
    /// Deck is the default implementation of IDeck which can be handled as a type of collection
    /// </summary>
    public class Deck : List<ICard>, IDeck
    {

        /// <summary>
        /// makes the total possible unique card value quantity known
        /// </summary>
        public static int MAX_CARD_COUNT = Enum.GetNames(typeof(Suit)).Length * Enum.GetNames(typeof(FaceValue)).Length;

        /// <summary>
        /// This constructor should be the only way to instantiate this class
        /// </summary>
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

        /// <summary>
        /// This override makes printing the deck contents more readable and is also
        /// used for the hash value override
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Uses the default value of the ShuffleStyle to shuffle the cards
        /// </summary>
        public void Shuffle()
        {
            this.Shuffle(ShuffleStyle.DEFAULT);
        }


        /// <summary>
        /// The main method for shuffling which takes an IShuffleStyle implementation and
        /// uses it's shuffle machine to do the work
        /// </summary>
        /// <param name="shuffleStyle"></param>
        public void Shuffle(IShuffleStyle shuffleStyle)
        {
            shuffleStyle.Machine(this);
        }

        /// <summary>
        /// Makes it possible to use many IShuffleStyle implementations to shuffle sequentially
        /// </summary>
        /// <param name="shuffleStyles"></param>
        public void Shuffle(ICollection<IShuffleStyle> shuffleStyles)
        {
            foreach(IShuffleStyle shuffleStyle in shuffleStyles)
            {
                shuffleStyle.Machine(this);
            }
        }

        /// <summary>
        /// Makes it possible to compare by the value of the deck contents
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Uses the string value to ensure unique card sets (could be two of the same value, not recommended)
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

    }


    /// <summary>
    /// Default implementation of the ICard interface
    /// </summary>
    public class Card : ICard
    {
        /// <summary>
        /// The suit of the card
        /// </summary>
        public Suit Suit { get; protected set; }

        /// <summary>
        /// The face value of the card minus the suit
        /// </summary>
        public FaceValue FaceValue { get; protected set; }

        /// <summary>
        /// Use this constructor to create a new card
        /// </summary>
        /// <param name="suit"></param>
        /// <param name="cardval"></param>
        public Card(Suit suit, FaceValue cardval)
        {
            this.Suit = suit;
            this.FaceValue = cardval;
        }

        /// <summary>
        /// Using this to sort the cards using standard Sort method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Override to make a more readable string output of the value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "" + this.FaceValue  + "_OF_" + this.Suit;
        }

        /// <summary>
        /// Makes two cards with the same values equal to each other
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Uses the suit and facevalue to calculate unique hash 
        /// provided the values of Suit and FaceValue don't get out of hand
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int suitInt = (int)this.Suit + 1 * 100;
            int faceInt = (int)this.FaceValue + 1;
            int hashCode = suitInt + faceInt;
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// The one method required to implement your own style of shuffle
    /// </summary>
    public interface IShuffleStyle
    {
        void Machine(IList<ICard> deck);
    }


    /// <summary>
    /// A class to make it easy to select the available implementations of IShuffleStyle
    /// I had hoped to implement more but at this time it is what it is.  This link provided
    /// me with an interesting list and ideas for providing different styles of shuffling.
    /// 
    ///      http://www.pokerology.com/articles/how-to-shuffle-cards
    /// 
    /// </summary>
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

    /// <summary>
    /// Some reusable code to randomly split the deck not always at the exact middle
    /// </summary>
    public class HumanShuffleStyleHelper
    {
        public static double STANDARD_DECK_SPLIT_PRECISION = 0.2;
        public static double DEALER_FIRED_DECK_SPLIT_PRECISION = 0.7;
        public bool DEALER_IS_RIGHT_HANDED = true;
        public Random random = new Random((int)DateTime.Now.Ticks);
        public double splitPrecision = STANDARD_DECK_SPLIT_PRECISION; // Percentage of the middle the deck will be split at 0.0 is absolute precision

        /// <summary>
        /// Provided the number of cards this method will calculate where to split the deck
        /// with some randomness with possible configurable precision
        /// </summary>
        /// <param name="cardCount"></param>
        /// <returns></returns>
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

    /// <summary>
    /// The Overhand Shuffle – This is the shuffle used by most people.  A good simple, lazy, sloppy shuffle.
    /// </summary>
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


    /// <summary>
    /// The Hindu Shuffle – A simple, quick and very elegant shuffle.One of my personal favourites.
    /// </summary>
    public class HinduShuffleStyle : IShuffleStyle
    {
        public void Machine(IList<ICard> deck)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// The Weave Shuffle – A very simple shuffle to perform and for those yet to master the riffle shuffle.
    /// </summary>
    public class WeaveShuffleStyle : IShuffleStyle
    {
        public void Machine(IList<ICard> deck)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// The Riffle Shuffle – This is a great way to shuffle cards and not as difficult as it looks.
    /// </summary>
    public class RiffleShuffleStyle : IShuffleStyle
    {
        public void Machine(IList<ICard> deck)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// The Table Riffle Shuffle – This is easier than the in the hands riffle shuffle, yet just as effective and elegant.
    /// </summary>
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

    /// <summary>
    /// TODO
    /// </summary>
    public class PortlandPowerhouseShuffleStyle : IShuffleStyle
    {
        public void Machine(IList<ICard> deck)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// TODO
    /// </summary>
    public class ShiftWiseFreeStyleShowOffShuffleStyle : IShuffleStyle
    {
        public void Machine(IList<ICard> deck)
        {
            throw new NotImplementedException();
        }
    }

    

}
