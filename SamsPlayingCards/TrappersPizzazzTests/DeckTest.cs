using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrappersPizzazz;
using System.Collections.Generic;

namespace TrappersPizzazzTests
{
    [TestClass]
    public class DeckTest
    {


        [TestMethod]
        public void TestDeckInstantiationIsNotNull()
        {
            Deck deck = new Deck();
            Assert.IsNotNull(deck);
            Assert.AreEqual(deck.Count, Deck.MAX_CARD_COUNT);
        }


        [TestMethod]
        public void TestShuffleDeckOfCards()
        {
            Deck sortedDeck = new Deck();
            Deck shuffledDeck = new Deck();
            shuffledDeck.Shuffle();
            Assert.AreNotEqual<IDeck>(sortedDeck, shuffledDeck);
        }


        [TestMethod]
        public void TestSameCardsAreEqual()
        {
            ICard card1 = new Card(Suit.CLUBS, FaceValue.ACE);
            ICard card2 = new Card(Suit.CLUBS, FaceValue.ACE);
            Assert.AreEqual<ICard>(card1, card2);
        }

        [TestMethod]
        public void TestDifferentCardsAreNotEqual()
        {
            ICard card1 = new Card(Suit.CLUBS, FaceValue.ACE);
            ICard card2 = new Card(Suit.DIAMONDS, FaceValue.ACE);
            Assert.AreNotEqual<ICard>(card1, card2);
        }

        [TestMethod]
        public void TestCardRemovedFromDeckEquality()
        {
            IDeck deck1 = new Deck();
            IDeck deck2 = new Deck();
            deck2.RemoveAt(0);
            Assert.AreNotEqual<IDeck>(deck1, deck2);
        }


        [TestMethod]
        public void TestDifferentCardsAddedToSet()
        {
            int EXPECTED_COUNT = 2;
            ISet<ICard> cardSet = new SortedSet<ICard>();
            ICard card1 = new Card(Suit.HEARTS, FaceValue.EIGHT);
            ICard card2 = new Card(Suit.HEARTS, FaceValue.SEVEN);
            cardSet.Add(card1);
            cardSet.Add(card2);
            Assert.AreEqual(cardSet.Count, EXPECTED_COUNT);
            ICard card3 = new Card(Suit.HEARTS, FaceValue.EIGHT); // New instance but same value as card1
            cardSet.Add(card3);  // Adding this card to the set should not increase the set count because value already exists.
            Assert.AreEqual(cardSet.Count, EXPECTED_COUNT);
        }


        [TestMethod]
        public void TestNewDecksAreEqual()
        {
            IDeck deck1 = new Deck();
            IDeck deck2 = new Deck();
            Assert.AreEqual<IDeck>(deck1, deck2);
        }


        [TestMethod]
        public void TestDeckWithExtraCardIsNotEqual()
        {
            IDeck deck1 = new Deck();
            IDeck deck2 = new Deck();
            deck2.Add(new Card(Suit.DIAMONDS, FaceValue.EIGHT));
            Assert.AreNotEqual<IDeck>(deck1, deck2);
        }


        [TestMethod]
        public void TestNewDeckDifferentThanShuffledDeck()
        {
            IDeck deck1 = new Deck();
            IDeck shuffledDeck = new Deck();
            shuffledDeck.Shuffle();
            Assert.AreNotEqual<IDeck>(deck1, shuffledDeck);
        }


        [TestMethod]
        public void TestDuplicateCardsDontExistInNewDeck()
        {
            IDeck deck = new Deck();
            ISet<ICard> cardSet = new HashSet<ICard>(deck);
            Assert.AreEqual(cardSet.Count, deck.Count);
        }


        [TestMethod]
        public void TestNoDuplicateCardsInShuffledDeck()
        {
            IDeck deck = new Deck();
            deck.Shuffle();
            ISet<ICard> cardSet = new HashSet<ICard>(deck);
            Assert.AreEqual(cardSet.Count, deck.Count);
        }

    }

}
