using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FarmSImulator
{
    public partial class Form1 : Form
    {
        Dictionary<CheckBox, Cell> field = new Dictionary<CheckBox, Cell>();
        Dictionary<string, int> Prices = new Dictionary<string, int>();
        private int day = 0;
        private int StandartTick = 20;
        private int money = 30;

        public Form1()
        {
            InitializeComponent();

            foreach (CheckBox cb in tableLayoutPanel1.Controls)
                field[cb] = new Cell();

            InitializePrices();
        }

        private void InitializePrices()
        {
            Prices.Add("plant", -2);
            Prices.Add("yellow", 3);
            Prices.Add("red", 5);
            Prices.Add("brown", -1);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.Checked) Plant(cb);
            else Harvest(cb);
        }


        private void Plant(CheckBox cb)
        {
            if (checkWallet(Prices["plant"]))
            {
                field[cb].Plant();
                UpdateBox(cb);
            }
        }

        private bool checkWallet(int price)
        {
            if (money<-price) return false;
            money += price;
            return true;
        }


        private void Harvest(CheckBox cb)
        {
            if (Pay(field[cb])) { 
                field[cb].Harvest();
                UpdateBox(cb);
            }
        }

        private bool Pay(Cell product)
        {
            int profit = 0;
            switch(product.state){
                case CellState.Immature:
                    profit = Prices["yellow"];
                    break;
                case CellState.Mature:
                    profit = Prices["red"];
                    break;
                case CellState.Rotten:
                    if (!checkWallet(Prices["brown"]))
                        return false;
                    break;
                default:
                    break;
            }
            money += profit;
            return true;
        }

        private void NextStep(CheckBox cb)
        {
            field[cb].NextStep();
            UpdateBox(cb);
        }
        private void UpdateBox(CheckBox cb)
        {
            Color c = Color.White;
            switch (field[cb].state)
            {
                case CellState.Planted:
                    c = Color.Black;
                    break;
                case CellState.Green:
                    c = Color.Green;
                    break;
                case CellState.Immature:
                    c = Color.Yellow;
                    break;
                case CellState.Mature:
                    c = Color.Red;
                    break;
                case CellState.Rotten:
                    c = Color.Brown;
                    break;
            }
            cb.BackColor = c;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            foreach (CheckBox cb in tableLayoutPanel1.Controls)
                NextStep(cb);
            day++;
            labDay.Text = "Day: " + day;
            moneyLabel.Text = "Moneu: " + money;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            timer1.Stop();
            timer1.Interval = Convert.ToInt32(StandartTick*(10-trackBar1.Value));
            timer1.Start();
        }

    }
    enum CellState
    {
        Empty,
        Planted,
        Green,
        Immature,
        Mature,
        Rotten
    }
    
    class Cell
    {
        public CellState state = CellState.Empty;
        private int progress = 0;

        const int prPlanted = 20;
        const int prGreen = 100;
        const int prImmature = 120;
        const int prMature = 140;


        public void Plant()
        {
            state = CellState.Planted;
        }

        public void Harvest()
        {
            state = CellState.Empty;
            progress = 0;
        }

        public void NextStep()
        {
            if ((state != CellState.Empty) && (state != CellState.Rotten))
            {
                progress++;
                if ((progress == prPlanted) || (progress == prGreen) || (progress == prImmature) || (progress == prMature))
                    state++;
            }
        }
    }
}