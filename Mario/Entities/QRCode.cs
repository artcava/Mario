using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mario.Characters
{
    public class QRCode : CharacterEntity
    {
        public QRCode(GameObject gameObject) : base(gameObject)
        {
            this.Name = "qrcode";
            //this.EntitySpriteSheet = ResourceManager.Instance.GetSpriteSheet("qrcode");
            //this.EntitySpriteSheet.DefineFrames(Direction.NONE, new int[] { 0 });
            this.Facing = Direction.NONE;
            this.IsPlayer = false;
            this.IsStatic = true;
            this.Acceleration = 0;
            this.AllowOffscreen = false;
        }
    }
}
