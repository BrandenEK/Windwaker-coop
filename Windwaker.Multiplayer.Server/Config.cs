﻿using Newtonsoft.Json;
using System;

namespace Windwaker.Multiplayer.Server
{
    /// <summary>
    /// The server settings
    /// </summary>
    [Serializable]
    public class Config
    {
        /// <summary>
        /// The port used when creating a new server
        /// </summary>
        [JsonProperty] public readonly int port;

        /// <summary>
        /// The max number of players allowed in a server
        /// </summary>
        [JsonProperty] public readonly int maxPlayers;

        /// <summary>
        /// Whether or not to allow players to use cheats
        /// </summary>
        [JsonProperty] public readonly bool allowCheats;

        /// <summary>
        /// Gets the default settings
        /// </summary>
        public Config()
        {
            port = 8989;
            maxPlayers = 10;
            allowCheats = true;
        }
    }
}