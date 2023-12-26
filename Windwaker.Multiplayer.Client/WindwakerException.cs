using System;

namespace Windwaker.Multiplayer.Client
{
    internal class WindwakerException : Exception
    {
        public WindwakerException(string message) : base(message) { }
    }
}
