using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Patte_pe_patta.Deck
{
    [CreateAssetMenu(fileName = "NewDeck", menuName = "Card Game/Deck")]
    public class DeckDataSO : ScriptableObject
    {
        public Card[] ClubCards;
        public Card[] DiamondCards;
        public Card[] HeartCards;
        public Card[] SpadeCards;

        private void OnValidate()
        {
            // Check for suit size limits
            if (ClubCards.Length > 13)
                Debug.LogWarning("Club has more than 13 cards.");
            if (DiamondCards.Length > 13)
                Debug.LogWarning("Diamond has more than 13 cards.");
            if (HeartCards.Length > 13)
                Debug.LogWarning("Heart has more than 13 cards.");
            if (SpadeCards.Length > 13)
                Debug.LogWarning("Spade has more than 13 cards.");

            // Collect all cards into one list
            List<Card> allCards = new();
            allCards.AddRange(ClubCards);
            allCards.AddRange(DiamondCards);
            allCards.AddRange(HeartCards);
            allCards.AddRange(SpadeCards);

            // Check for nulls
            if (allCards.Any(card => card == null))
            {
                Debug.LogWarning("Some cards are missing (null references found).");
                return;
            }
        }
    }
}