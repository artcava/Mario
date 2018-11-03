using GameEngine;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Windows.Forms;
using DimeXplorer;
using System.Linq;

namespace Mario
{
    public class StartScene : Scene
    {
        Text text;

        public StartScene (GameObject gameObject): base(gameObject)
        {
            this.BackgroundColor = Color.Black;
        }

        public override void Initialize()
        {
            Font arial = new Font(@"resources\arial.ttf");

            text = new Text("", arial);
            text.Position = new Vector2f(0, 0);
            text.CharacterSize = 30;

        }

        public override void Reset()
        {
            Characters.Mario mario = new Characters.Mario(this._gameObject);
            mario.X = 430;
            mario.Y = 360;
            mario.sprite.TextureRect = mario.EntitySpriteSheet.GetFirstSprite(Direction.RIGHT);
            this.Entities.Add(mario);

            base.Reset();
        }

        public override void HandleKeyPress(SFML.Window.KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.P) dimeOk = true;
            if (e.Code == Keyboard.Key.Space)
            {
                if (dimeOk)
                    _gameObject.SceneManager.StartScene("play");
            }

            if (e.Code == Keyboard.Key.Return)
            {
                Form frm = new frmTransaction();
                frm.ShowDialog();
            }

            if (e.Code == Keyboard.Key.Escape)
                this._gameObject.Window.Close();

            base.HandleKeyPress(e);
        }

        public override void Update()
        {
            string t = "MARIO";
            text.DisplayedString = t;
            text.Position = new Vector2f(100, 50);
            text.Draw(this._gameObject.Window, RenderStates.Default);

            t = ((MainScene)_gameObject.SceneManager.GetScene("play")).Score.ToString("000000");
            text.DisplayedString = t;
            text.Position = new Vector2f(100, 80);
            text.Draw(this._gameObject.Window, RenderStates.Default);

            t = "x " + ((MainScene)_gameObject.SceneManager.GetScene("play")).PlayerLives.ToString("00");
            text.DisplayedString = t;
            text.Position = new Vector2f(300, 80);
            text.Draw(this._gameObject.Window, RenderStates.Default);

            t = "WORLD";
            text.DisplayedString = t;
            text.Position = new Vector2f(530, 50);
            text.Draw(this._gameObject.Window, RenderStates.Default);

            t = "1-1";
            text.DisplayedString = t;
            text.Position = new Vector2f(560, 80);
            text.Draw(this._gameObject.Window, RenderStates.Default);

            t = "TIME";
            text.DisplayedString = t;
            text.Position = new Vector2f(830, 50);
            text.Draw(this._gameObject.Window, RenderStates.Default);

            t = "WORLD 1-1";
            text.DisplayedString = t;
            text.Position = new Vector2f(420, 300);
            text.Draw(this._gameObject.Window, RenderStates.Default);

            t = "x " + ((MainScene)_gameObject.SceneManager.GetScene("play")).PlayerLives.ToString("00");
            text.DisplayedString = t;
            text.Position = new Vector2f(500, 380);
            text.Draw(this._gameObject.Window, RenderStates.Default);

            if (!dimeOk)
                dimeOk = CheckPayment();

            t = (dimeOk) ? "[SPACE] To Begin": "[RETURN] To UnLock";
            text.DisplayedString = t;
            text.Position = new Vector2f(380, 580);
            text.Draw(this._gameObject.Window, RenderStates.Default);

        }

        public override void Draw()
        {
            foreach (CharacterEntity c in Entities)
                c.Draw();
        }

        public override void DrawBackground()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CheckPayment()
        {
            var walletPrivate = Wallet.GetPrivateWallet();
            if (walletPrivate == null) return false;

            var walletPublic = Wallet.GetPublicWallet();
            if (walletPublic == null) return false;

            var explorer = new Explorer();
            var txList = explorer.getTransactionsByAddress(1, 20, walletPublic);
            if (txList != null)
            {
                foreach (var dTx in txList.Data)
                {
                    var tx = explorer.getTransaction(dTx.TransactionHash);
                    if (tx == null) continue;
                    var wPub = tx.AddressOutputs.FirstOrDefault(w => w.WalletAddress.Equals(walletPublic));
                    if (wPub == null) return false;

                    var wPriv = tx.AddressInputs.FirstOrDefault(w => w.WalletAddress.Equals(walletPrivate));
                    if (wPriv == null) return false;

                                if (wPub.Value >= 100M) return true;
                }
            }
            return false;
        }
    }
}
