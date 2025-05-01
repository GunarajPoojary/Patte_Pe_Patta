using System.Collections;
using Patte_pe_patta.Deck;
using Patte_pe_patta.Utility;
using UnityEngine;
using UnityEngine.Splines;

namespace Patte_pe_patta
{
    public class GameService : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _bgRenderer;
        [SerializeField] private SpriteRenderer _tableRenderer;
        [SerializeField] private DeckDataSO _deckDataSO;
        [SerializeField] private SplineContainer _p1SplineContainer;
        [SerializeField] private SplineContainer _p2SplineContainer;

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

        public void DistributeCards() => DeckService.DistributeCards(_p1SplineContainer, _p2SplineContainer);

        public void Shuffle() => DeckService.ShuffleCards();
    }
}