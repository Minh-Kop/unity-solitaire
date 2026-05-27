using UnityEngine;

namespace Core
{
    public enum CardType
    {
        String,
        Image,
        Cover,
    }

    [System.Serializable]
    public class Card
    {
        public Card(CardType cardType, string type)
        {
            CardType = cardType;
            Type = type;
        }

        public Card(CardType cardType, string type, string content)
        {
            CardType = cardType;
            Type = type;
            StringContent = content;
        }

        public Card(CardType cardType, string type, Sprite sprite)
        {
            CardType = cardType;
            Type = type;
            SpriteContent = sprite;
        }

        public bool IsFaceUp { get; set; }
        public CardType CardType { get; private set; }
        public string Type { get; private set; }
        public string StringContent { get; private set; }
        public Sprite SpriteContent { get; private set; }
    }
}
