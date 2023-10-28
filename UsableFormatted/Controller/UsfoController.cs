using System;
using System.Collections.Generic;
using Usfo;

namespace UsableFormatted.Controller
{
    internal static class UsfoController
    {
        private static InteropEngine? _interopEngine = null;

        // Possibility to use also another engine
        internal static UsfoEngine GetEngine()
        {
            if (_interopEngine == null)
                _interopEngine = new InteropEngine();

            return _interopEngine;
        }
    }
}