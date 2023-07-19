using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windwaker.Multiplayer.Client
{
    internal class WindwakerProgress
    {
        public byte stageId = 0xFF;

        //public byte swordId = 0xFF;
        //public byte swordBitfield = 0;

        //public byte shieldId = 0xFF;
        //public byte shieldBitfield = 0;


        // Inventory

        public bool telescope;
        public bool sail;
        public bool windWaker;
        public bool grapplingHook;
        public bool spoilsBag;
        public bool boomerang ;
        public bool dekuLeaf;

        public byte tingleTuner = 0;
        public byte pictoBox = 0;
        public byte ironBoots = 0;
        public byte magicArmor = 0;
        public byte baitBag = 0;
        public byte bow = 0;
        public byte bombs = 0;

        public byte bottle1 = 0;
        public byte bottle2 = 0;
        public byte bottle3 = 0;
        public byte bottle4 = 0;
        public byte deliveryBag = 0;
        public byte hookshot = 0;
        public byte skullHammer = 0;

        // Equipment

        public byte sword = 0;
        public byte shield = 0;

        public byte powerBracelets = 0;
        public byte piratesCharm = 0;
        public byte herosCharm = 0;
        public byte tingleStatues = 0;

        public byte bagContents = 0;

        public byte songs = 0;
        public byte pearls = 0;
        public byte triforceShards = 0;

        // Capcities

        public byte maxHealth = 0;
        public byte maxMagic = 0;
        public byte wallet = 0;
        public byte quiver = 0;
        public byte bombBag = 0;

        // Charts

        public byte chartsOwned = 0;
        public byte chartsOpened = 0;
        public byte chartsLooted = 0;
        public byte chartsDeciphered = 0;

        public byte sectors = 0;

        public byte stages = 0;

        public byte events = 0;
    }
}
