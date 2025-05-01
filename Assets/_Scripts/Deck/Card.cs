using UnityEngine;

namespace Patte_pe_patta.Deck
{
    public class Card : MonoBehaviour
    {
        private SpriteRenderer[] _renderers;

        [field: SerializeField] public CardType Type { get; private set; }

        private void Awake() => _renderers = GetComponentsInChildren<SpriteRenderer>();

        public void UpdateSortingOrders(int sortOrder)
        {
            if (_renderers == null) return;

            for (int i = 0; i < _renderers.Length; i++)
                _renderers[i].sortingOrder = sortOrder - i;
        }
    }
}