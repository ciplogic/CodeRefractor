using System;

namespace SimpleAdditions
{
    class NBody
    {
        CallEvent _onEvent;
        void Logic()
        {
            _onEvent = EventHandle;

            _onEvent(2);
        }
        public static void Main()
        {
            var nBody = new NBody();
            nBody.Logic();
        }

        private void EventHandle(int a)
        {
            Console.Write("Works");

            Console.Write(a);
        }
    }
}