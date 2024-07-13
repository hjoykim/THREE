
namespace THREE {

    [Serializable]
    public struct Rectangle
    {
        public static readonly Rectangle Empty;

        private int x;
        private int y;
        private int width;
        private int height;

        public Rectangle(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public int X
        {
            readonly get => x;
            set => x = value;
        }

        public int Y
        {
            readonly get => y;
            set => y = value;
        }

        public int Width
        {
            readonly get => width;
            set => width = value;
        }


        public int Height
        {
            readonly get => height;
            set => height = value;
        }

        public readonly int Left => X;

        public readonly int Top => Y;

        public readonly int Right => unchecked(X + Width);

        public readonly int Bottom => unchecked(Y + Height);

        public readonly bool IsEmpty => height == 0 && width == 0 && x == 0 && y == 0;


    }
}
