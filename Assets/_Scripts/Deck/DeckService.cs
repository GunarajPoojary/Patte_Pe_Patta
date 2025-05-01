using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Splines;
using System.Threading.Tasks;
using System.Threading;
using System;

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
        private Transform _deckContainer;
        private bool _isPlayingShuffleAnim = false;
        private bool _hasShuffled;
        private bool _canDistribute = false;
        private bool _hasDistributed = false;

        private float _delayPerCard = 0.01f;
        private float _moveDistance = 0.8f;
        private float _moveDuration = 0.5f;
        private int _cardIndex;

        private List<Card> _playerOneHandCards = new();
        private List<Card> _playerTwoHandCards = new();
        private CancellationTokenSource _distributeCts;

        public DeckService(DeckDataSO deck)
        {
            _deck = new List<Card>();
            _cardIndex = 0;
            _hasDistributed = false;

            CreateDeck(deck);
        }

        private void CreateDeck(DeckDataSO deckDataSO)
        {
            _deckContainer = new GameObject("DeckContainer").transform;

            for (int i = 0; i < 13; i++)
            {
                var clubCard = UnityEngine.Object.Instantiate(deckDataSO.ClubCards[i], _deckContainer);
                _deck.Add(clubCard);

                var diamondCard = UnityEngine.Object.Instantiate(deckDataSO.DiamondCards[i], _deckContainer);
                _deck.Add(diamondCard);

                var heartCard = UnityEngine.Object.Instantiate(deckDataSO.HeartCards[i], _deckContainer);
                _deck.Add(heartCard);

                var spadeCard = UnityEngine.Object.Instantiate(deckDataSO.SpadeCards[i], _deckContainer);
                _deck.Add(spadeCard);
            }

            _deckContainer.Rotate(0, 0, -90);
        }

        public void ShuffleCards()
        {
            if (_isPlayingShuffleAnim || _hasDistributed || _hasShuffled || _deck == null) return;

            PlayShuffleAnim();

            for (int i = _deck.Count - 1; i >= 0; i--)
            {
                int j = UnityEngine.Random.Range(0, _deck.Count);
                (_deck[i], _deck[j]) = (_deck[j], _deck[i]);
            }

            for (int i = 0; i < _deck.Count; i++)
            {
                _deck[i].transform.SetSiblingIndex(i);
                Debug.Log(_deck[i].name);
            }
        }

        private void PlayShuffleAnim()
        {
            _isPlayingShuffleAnim = true;

            int half = _deck.Count / 2;

            Sequence shuffleSequence = DOTween.Sequence();

            for (int i = 0; i < half; i++)
            {
                Transform leftCard = _deck[i].transform;
                Transform rightCard = _deck[i + half].transform;

                float delay = i * _delayPerCard;

                // Move left card
                shuffleSequence.Insert(delay,
                    leftCard.DOMoveY(leftCard.position.y + _moveDistance, _moveDuration)
                        .SetEase(Ease.InCubic));

                // Move right card
                shuffleSequence.Insert(delay,
                    rightCard.DOMoveY(rightCard.position.y - _moveDistance, _moveDuration)
                        .SetEase(Ease.InCubic));
            }

            shuffleSequence.OnComplete(() => GatherCards());
        }

        private void GatherCards()
        {
            Sequence gatherSeq = DOTween.Sequence();

            for (int i = 0; i < _deck.Count; i++)
            {
                Transform card = _deck[i].transform;

                float delay = i * _delayPerCard;

                gatherSeq.Insert(delay, card.DOMove(Vector3.zero, _moveDuration)
                    .SetEase(Ease.InCubic));
            }

            gatherSeq.OnComplete(() =>
            {
                _canDistribute = true;
                _isPlayingShuffleAnim = false;
                _hasShuffled = true;
            });
        }

        public async void DistributeCards(SplineContainer p1SplineContainer, SplineContainer p2SplineContainer)
        {
            // Cancel any ongoing distribution
            if (_distributeCts != null)
            {
                _distributeCts.Cancel();
                _distributeCts.Dispose();
            }

            _distributeCts = new CancellationTokenSource();
            try
            {
                await DistributeCardsAsync(p1SplineContainer, p2SplineContainer, _distributeCts.Token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Card distribution was canceled.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error distributing cards: {ex.Message}");
            }
        }

        private async Task DistributeCardsAsync(SplineContainer p1SplineContainer, SplineContainer p2SplineContainer, CancellationToken cancellationToken)
        {
            int maxHandSize = 32;

            for (int i = 0; i < maxHandSize * 2; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!_canDistribute || _cardIndex >= _deck.Count) return;

                if (_cardIndex % 2 == 0)
                {
                    if (_playerOneHandCards.Count >= maxHandSize) return;

                    _playerOneHandCards.Add(_deck[_cardIndex]);
                    UpdateCardPosition(maxHandSize, p1SplineContainer, _playerOneHandCards);
                }
                else
                {
                    if (_playerTwoHandCards.Count >= maxHandSize) return;

                    _playerTwoHandCards.Add(_deck[_cardIndex]);
                    UpdateCardPosition(maxHandSize, p2SplineContainer, _playerTwoHandCards);
                }

                _cardIndex++;

                await Task.Delay(50, cancellationToken); // 50ms delay (equivalent to 0.05f seconds in coroutine)
            }

            _hasDistributed = true;
            _canDistribute = false;
            KillAnim();
            Debug.Log("Card distribution complete!");
        }

        public void UpdateCardPosition(int maxHandSize, SplineContainer splineContainer, List<Card> handCards)
        {
            if (handCards.Count == 0) return;

            float cardSpacing = 1f / maxHandSize;
            float firstCardPosition = 0.5f - (handCards.Count - 1) * cardSpacing / 2;
            Spline spline = splineContainer.Spline;

            for (int i = 0; i < handCards.Count; i++)
            {
                float p = firstCardPosition + i * cardSpacing;

                Vector3 splinePos = spline.EvaluatePosition(p);
                Vector3 forward = spline.EvaluateTangent(p);
                Vector3 up = spline.EvaluateUpVector(p);

                Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);

                handCards[i].transform.DOMove(splinePos, 0.25f);
                handCards[i].transform.DORotateQuaternion(rotation, 0.25f);
                handCards[i].UpdateSortingOrders(i);
            }
        }

        public void KillAnim() => DOTween.KillAll();

        public void ResetDeck()
        {
            _playerOneHandCards.Clear();
            _playerTwoHandCards.Clear();
            _cardIndex = 0;

            // Cancel any ongoing distribution
            if (_distributeCts != null)
            {
                _distributeCts.Cancel();
                _distributeCts.Dispose();
                _distributeCts = null;
            }
        }
    }
}