using Patte_pe_patta.Deck;
using Patte_pe_patta.Utility;
using UnityEngine;

namespace Patte_pe_patta
{
    public class GameService : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _bgRenderer;
        [SerializeField] private SpriteRenderer _tableRenderer;
        [SerializeField] private DeckDataSO _deckDataSO;
        public DeckService DeckService;

        private void Awake()
        {
            ResponsiveSpriteRenderer.FitToSafeAreaHeight(_tableRenderer);
            ResponsiveSpriteRenderer.FitToFullScreen(_bgRenderer);
        }

        private void Start()
        {
            DeckService = new DeckService(_deckDataSO);
        }

        public void Shuffle() => DeckService.ShuffleCards();
    }
}