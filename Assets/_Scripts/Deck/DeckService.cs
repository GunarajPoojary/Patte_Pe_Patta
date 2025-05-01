using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
        private int _shuffleCount;
        private int _maxShuffleCount = 3;
        private bool _isShuffling = false;
        private bool _isGathering = false;

        private float _delayPerCard = 0.01f;
        private float _moveDistance = 0.5f;
        private float _moveDuration = 0.5f;

        public DeckService(DeckDataSO deck)
        {
            _deck = new List<Card>();

            _shuffleCount = 0;

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
            if (_deck == null)
            {
                Debug.LogError("Deck Serivce has not been initialized");
                return;
            }

            if (_shuffleCount == _maxShuffleCount)
            {
                Debug.Log("You've reache max shuffle count");
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

            _shuffleCount++;
        }

        private void PlayShuffleAnim()
        {
            if (_isShuffling || _isGathering) return;

            _isShuffling = true;

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

            shuffleSequence.OnComplete(() => _isShuffling = false);
        }

        public void GatherCards()
        {
            if (_isShuffling || _isGathering) return;

            if (_shuffleCount == 0)
            {
                Debug.Log("You haven't shuffled the deck");
                return;
            }
            _isGathering = true;

            _shuffleCount = 0;

            Sequence gatherSeq = DOTween.Sequence();

            for (int i = 0; i < _deck.Count; i++)
            {
                Transform card = _deck[i].transform;

                float delay = i * _delayPerCard;

                gatherSeq.Insert(delay, card.DOMove(Vector3.zero, _moveDuration)
                    .SetEase(Ease.InCubic));
            }

            gatherSeq.OnComplete(() => _isGathering = false);
        }

        public void DistributeCards()
        {

        }

        public void KillAnim() => DOTween.KillAll();
    }
}