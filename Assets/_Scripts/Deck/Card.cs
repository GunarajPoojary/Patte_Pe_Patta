using UnityEngine;

namespace Patte_pe_patta.Deck
{
    public class Card : MonoBehaviour
    {
        [field: SerializeField] public CardType Type { get; private set; }

        public override bool Equals(object obj)
        {
            if (obj is Card otherCard)
                return Type == otherCard.Type;
            return false;
        }

        public override int GetHashCode() => Type.GetHashCode();
    }
}