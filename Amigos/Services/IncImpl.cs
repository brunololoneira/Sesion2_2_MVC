namespace Amigos.Services
{
    public class IncImpl : IInc
    {
        private int x = 0;

        public int Inc()
        {
            x++;
            return x;
        }
    }
}
