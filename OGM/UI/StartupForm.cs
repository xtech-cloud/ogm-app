using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using XTC.oelMVCS;

namespace OGM
{
    public partial class StartupForm : Form
    {
        public AppForm appForm { get; set; }
        public Config config { get; set; }
        public class Favorite
        {
            public string remote { get; set; }
            public string username { get; set; }
        }

        private List<Favorite> favorites = new List<Favorite>();

        public StartupForm()
        {
            this.FormClosed += onFormClosed;
            InitializeComponent();
            loadFavorite();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Favorite favorite = favorites.Find((_item) =>
            {
                return _item.remote.Equals(_item);
            });
            if(null == favorite)
            {
                favorite = new Favorite();
                favorite.remote = this.tbRemote.Text;
            }
            favorite.username = this.tbUsername.Text;

            favorites.Add(favorite);

            string json = System.Text.Json.JsonSerializer.Serialize(favorites);
            File.WriteAllText("./favorites.json", json);
        }

        private void loadFavorite()
        {
            try
            {
                string json = File.ReadAllText("./favorites.json");
                favorites = System.Text.Json.JsonSerializer.Deserialize<List<Favorite>>(json);

            }catch(System.Exception ex)
            {

            }

            foreach(Favorite favorite in favorites)
            {
                this.cbFavorite.Items.Add(favorite.remote);
            }
        }

        private void cbFavorite_SelectedIndexChanged(object sender, EventArgs e)
        {
            Favorite favorite = favorites.Find((_item) =>
            {
                return _item.remote.Equals(this.cbFavorite.Text);
            });
            if (null == favorite)
                return;
            this.tbRemote.Text = favorite.remote;
            this.tbUsername.Text = favorite.username;
        }

        private void btnSignin_Click(object sender, EventArgs e)
        {
            ConfigSchema schema = new ConfigSchema();
            schema.domain = this.tbRemote.Text;
            config.Merge(System.Text.Json.JsonSerializer.Serialize(schema));
            this.Hide();
            appForm.Show();
        }

        private void onFormClosed(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
