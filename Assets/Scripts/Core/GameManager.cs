using System.Collections.Generic;
using Piles;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Piles — kéo thả từ Scene vào")]
        [SerializeField]
        private StockPile _stock;

        [SerializeField]
        private WastePile _waste;

        [SerializeField]
        private FoundationPile[] _foundations; // 4 phần tử

        [SerializeField]
        private TableauPile[] _tableaux; // 4 phần tử

        [Header("Card Prefab & Sprites")]
        [SerializeField]
        private GameObject _textCardPrefab;

        [SerializeField]
        private GameObject _spriteCardPrefab;

        [SerializeField]
        private GameObject _coverCardPrefab;

        [Header("Layout (World Space)")]
        [SerializeField]
        private float cardWidth = 0.7f; // Khoảng cách ngang giữa các cột

        [SerializeField]
        private float tableauStartX = -2.1f; // X của cột Tableau đầu tiên

        [SerializeField]
        private float tableauStartY = 1.5f; // Y bắt đầu của hàng Tableau

        [SerializeField]
        private float _stockX = -2.1f; // X của Stock

        [SerializeField]
        private float _wasteX = -1.4f; // X của Waste

        [SerializeField]
        private float foundationStartX = 0.7f; // X của Foundation đầu tiên

        [SerializeField]
        private float topRowY = 3f; // Y của hàng Stock/Foundation

        public static int MoveCount { get; set; } = 50;

        // [Header("Systems")]
        // public WinChecker winChecker;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            // PositionPiles();
            DealCards();
        }

        // ─── Đặt vị trí các Pile theo world space ───────────────────────────────

        private void PositionPiles()
        {
            // Stock & Waste
            _stock.transform.position = new Vector3(_stockX, topRowY, 0);
            _waste.transform.position = new Vector3(_wasteX, topRowY, 0);

            // 4 Foundation (góc phải)
            for (var i = 0; i < _foundations.Length; i++)
            {
                _foundations[i].transform.position = new Vector3(
                    foundationStartX + i * cardWidth,
                    topRowY,
                    0
                );
            }

            // 7 cột Tableau
            for (var i = 0; i < _tableaux.Length; i++)
            {
                _tableaux[i].transform.position = new Vector3(
                    tableauStartX + i * cardWidth,
                    tableauStartY,
                    0
                );
            }
        }

        // ─── Tạo & chia bài ─────────────────────────────────────────────────────
        private void DealCards()
        {
            var deck = CreateDeck();
            Shuffle(deck);

            var index = 0;

            // Chia vào 7 cột Tableau
            for (var col = 0; col < _tableaux.Length; col++)
            {
                for (var row = 0; row <= col; row++)
                {
                    var card = deck[index++];
                    if (row == col)
                    {
                        card.IsFaceUp = true; // Lá trên cùng lật ngửa
                        _tableaux[col].FirstFaceUpIndex = row;
                    }

                    var view = SpawnCard(card);
                    _tableaux[col].AddCard(view);
                }

                _tableaux[col].ArrangeCards();
            }

            // Phần còn lại vào Stock
            for (var i = index; i < deck.Count; i++)
            {
                var view = SpawnCard(deck[i]);
                _stock.AddCard(view);
            }

            _stock.ArrangeCards();
        }

        // ─── Helpers ────────────────────────────────────────────────────────────

        private List<Card> CreateDeck()
        {
            var deck = new List<Card>();
            deck.Add(new CoverCard("Animals", 3));
            deck.Add(new Card(CardType.Text, "Animals", "Cat"));
            deck.Add(new Card(CardType.Text, "Animals", "Dog"));
            deck.Add(new Card(CardType.Text, "Animals", "Bird"));

            deck.Add(new CoverCard("Colors", 5));
            deck.Add(new Card(CardType.Text, "Colors", "Red"));
            deck.Add(new Card(CardType.Text, "Colors", "Green"));
            deck.Add(new Card(CardType.Text, "Colors", "Blue"));
            deck.Add(new Card(CardType.Text, "Colors", "Purple"));
            deck.Add(new Card(CardType.Text, "Colors", "Yellow"));

            deck.Add(new CoverCard("Fruits", 6));
            deck.Add(new Card(CardType.Text, "Fruits", "Orange"));
            deck.Add(new Card(CardType.Text, "Fruits", "Starfruit"));
            deck.Add(new Card(CardType.Text, "Fruits", "Blueberry"));
            deck.Add(new Card(CardType.Text, "Fruits", "Mango"));
            deck.Add(new Card(CardType.Text, "Fruits", "Apple"));
            deck.Add(new Card(CardType.Text, "Fruits", "Peach"));

            return deck;
        }

        private void Shuffle(List<Card> deck)
        {
            for (var i = deck.Count - 1; i > 0; i--)
            {
                var j = Random.Range(0, i + 1);
                (deck[i], deck[j]) = (deck[j], deck[i]);
            }
        }

        private CardView SpawnCard(Card card)
        {
            // Spawn tại gốc, Pile.AddCard sẽ set position sau
            GameObject go;
            if (card is CoverCard)
            {
                go = Instantiate(_coverCardPrefab, Vector3.zero, Quaternion.identity);
            }
            else if (card.CardType == CardType.Text)
            {
                go = Instantiate(_textCardPrefab, Vector3.zero, Quaternion.identity);
            }
            else
            {
                go = Instantiate(_spriteCardPrefab, Vector3.zero, Quaternion.identity);
            }

            var view = go.GetComponent<CardView>();

            view.Setup(card);

            return view;
        }

        // ─── Gọi sau mỗi lần đặt bài thành công ────────────────────────────────

        public void OnCardPlaced()
        {
            // winChecker.CheckWin();
        }

        // ─── Auto move lên Foundation (double click) ────────────────────────────

        public void TryAutoMove(CardView card)
        {
            // Chỉ auto move lá trên cùng
            var currentPile = card.GetComponentInParent<Pile>();
            if (currentPile == null || currentPile.TopCard != card)
            {
                return;
            }

            foreach (var foundation in _foundations)
            {
                if (foundation.CanAccept(card))
                {
                    currentPile.RemoveCard(card);
                    currentPile.ArrangeCards();
                    foundation.AddCard(card);
                    foundation.ArrangeCards();

                    // Lật lá kế tiếp nếu từ Tableau
                    if (currentPile is TableauPile tp && tp.TopCard != null)
                    {
                        tp.TopCard.FlipFaceUp();
                    }

                    OnCardPlaced();
                    return;
                }
            }
        }
    }
}
