using System.Collections.Generic;
using UnityEngine;

namespace Patte_pe_patta.Deck
{
    public enum CardType
    {
        Ace = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13
    }

    public class DeckService
    {
        private List<Card> _deck;

        public DeckService(DeckDataSO deck)
        {
            _deck = new List<Card>();
            CreateDeck(deck);
        }

        private void CreateDeck(DeckDataSO deckDataSO)
        {
            Transform deckContainer = new GameObject("DeckContainer").transform;

            Transform diamondContainer = new GameObject("DiamondCardsContainer").transform;
            diamondContainer.SetParent(deckContainer);

            Transform spadeContainer = new GameObject("SpadeCardsContainer").transform;
            spadeContainer.SetParent(deckContainer);

            Transform clubContainer = new GameObject("ClubCardsContainer").transform;
            clubContainer.SetParent(deckContainer);

            Transform heartContainer = new GameObject("HeartCardsContainer").transform;
            heartContainer.SetParent(deckContainer);

            for (int i = 0; i < 13; i++)
            {
                var clubCard = Object.Instantiate(deckDataSO.ClubCards[i], clubContainer);
                _deck.Add(clubCard);

                var diamondCard = Object.Instantiate(deckDataSO.DiamondCards[i], diamondContainer);
                _deck.Add(diamondCard);

                var heartCard = Object.Instantiate(deckDataSO.HeartCards[i], heartContainer);
                _deck.Add(heartCard);

                var spadeCard = Object.Instantiate(deckDataSO.SpadeCards[i], spadeContainer);
                _deck.Add(spadeCard);
            }
        }

        public void ShuffleCards()
        {
            
        }

        public void DistributeCards()
        {

        }
    }
}