namespace Core
{
    public class CoverCard : Card
    {
        public CoverCard(string type, int maxCount)
            : base(CardType.Cover, type)
        {
            MaxCount = maxCount;
        }

        public int MaxCount { get; private set; }
        public int Count { get; set; }
    }
}
