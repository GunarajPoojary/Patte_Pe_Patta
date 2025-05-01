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

        private Coroutine _distribute;
        
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

        public void DistributeCards()
        {
            if (_distribute != null) StopCoroutine(_distribute);
            _distribute = StartCoroutine(DistributeCoroutine());
        }

        private IEnumerator DistributeCoroutine()
        {
            int maxHandSize = 32;

            for (int i = 0; i < maxHandSize * 2; i++)
            {
                DeckService.DealNextCard(maxHandSize, _p1SplineContainer, _p2SplineContainer);

                yield return new WaitForSeconds(0.03f); // Wait for animation to complete before next
            }

            Debug.Log("Card distribution complete!");
        }

        public void Shuffle() => DeckService.ShuffleCards();

        public void Gather() => DeckService.GatherCards();
    }
}