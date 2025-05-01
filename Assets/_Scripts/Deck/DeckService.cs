using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Splines;

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
        private bool _isPlayingGatherAnim = false;
        private bool _hasShuffled = false;
        private bool _hasGathered = true;
        private bool _canDistribute = false;
        private bool _hasDistributed = false;

        private float _delayPerCard = 0.01f;
        private float _moveDistance = 0.8f;
        private float _moveDuration = 0.5f;
        private int _cardIndex;

        private List<Card> _playerOneHandCards = new();
        private List<Card> _playerTwoHandCards = new();

        public DeckService(DeckDataSO deck)
        {
            _deck = new List<Card>();
            _cardIndex = 0;

            CreateDeck(deck);
        }

        private void CreateDeck(DeckDataSO deckDataSO)
        {
            _deckContainer = new GameObject("DeckContainer").transform;

            for (int i = 0; i < 13; i++)
            {
                var clubCard = Object.Instantiate(deckDataSO.ClubCards[i], _deckContainer);
                _deck.Add(clubCard);

                var diamondCard = Object.Instantiate(deckDataSO.DiamondCards[i], _deckContainer);
                _deck.Add(diamondCard);

                var heartCard = Object.Instantiate(deckDataSO.HeartCards[i], _deckContainer);
                _deck.Add(heartCard);

                var spadeCard = Object.Instantiate(deckDataSO.SpadeCards[i], _deckContainer);
                _deck.Add(spadeCard);
            }

            _deckContainer.Rotate(0, 0, -90);
        }

        public void ShuffleCards()
        {
            if (_isPlayingShuffleAnim || _isPlayingGatherAnim || !_hasGathered || _hasDistributed) return;

            if (_deck == null)
            {
                Debug.LogError("Deck Serivce has not been initialized");
                return;
            }

            for (int i = _deck.Count - 1; i >= 0; i--)
            {
                int j = Random.Range(0, _deck.Count);
                (_deck[i], _deck[j]) = (_deck[j], _deck[i]);
            }

            for (int i = 0; i < _deck.Count; i++)
            {
                _deck[i].transform.SetSiblingIndex(i);
                Debug.Log(_deck[i].name);
            }

            PlayShuffleAnim();
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

            shuffleSequence.OnComplete(() =>
            {
                _isPlayingShuffleAnim = false;
                _hasShuffled = true;
                _hasGathered = false;
            });
        }

        public void GatherCards()
        {
            if (_isPlayingShuffleAnim || _isPlayingGatherAnim || !_hasShuffled || _hasDistributed) return;

            _isPlayingGatherAnim = true;

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
                _isPlayingGatherAnim = false;
                _hasShuffled = false;
                _hasGathered = true;
            });
        }

        public void DealNextCard(int maxHandSize, SplineContainer p1SplineContainer, SplineContainer p2SplineContainer)
        {
            if (!_hasGathered || !_canDistribute || _cardIndex >= _deck.Count) return;

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
            _hasDistributed = true;
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
            // _deckState = DeckState.Idle;
        }
    }
}