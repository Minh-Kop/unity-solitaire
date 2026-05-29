using System;
using UnityEngine;

namespace Core
{
    public enum CardType
    {
        Text,
        Sprite,
        Cover,
    }

    [Serializable]
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
            TextContent = content;
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
        public string TextContent { get; private set; }
        public Sprite SpriteContent { get; private set; }
    }
}
