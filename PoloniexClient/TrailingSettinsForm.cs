﻿using CryptoMarketClient.Common;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CryptoMarketClient {
    public partial class TrailingSettinsForm : XtraForm {
        public TrailingSettinsForm() {
            InitializeComponent();
            this.imageComboBoxEdit1.Properties.AddEnum<ActionMode>();
        }

        TrailingSettings settings;
        public TrailingSettings Settings {
            get { return settings; }
            set {
                if(Settings == value)
                    return;
                settings = value;
                OnSettingsChanged();
            }
        }

        public TickerBase Ticker {
            get; set;
        }

        public TrailingCollectionControl CollectionControl { get; set; }
        OrderBookControl orderBook;
        public OrderBookControl OrderBookControl {
            get { return orderBook; }
            set {
                if(OrderBookControl == value)
                    return;
                OrderBookControl prev = OrderBookControl;
                orderBook = value;
                OnOrderBookChanged(prev);
            }
        }
        void OnOrderBookChanged(OrderBookControl prev) {
            if(prev != null) {
                prev.SelectedAskRowChanged -= OnSelectedAskRowChanged;
                prev.SelectedBidRowChanged -= OnSelectedBidRowChanged;
            }
            if(OrderBookControl != null) {
                OrderBookControl.SelectedAskRowChanged += OnSelectedAskRowChanged;
                OrderBookControl.SelectedBidRowChanged += OnSelectedBidRowChanged;
            }
        }

        private void OnSelectedBidRowChanged(object sender, SelectedOrderBookEntryChangedEventArgs e) {
            Settings.BuyPrice = e.Entry.Value;
        }

        void OnSelectedAskRowChanged(object sender, SelectedOrderBookEntryChangedEventArgs e) {
            Settings.BuyPrice = e.Entry.Value;
        }

        public EditingMode Mode { get; set; } = EditingMode.Add;

        void OnSettingsChanged() {
            this.trailingSettingsBindingSource.DataSource = Settings;
        }

        private void OnCancelClick(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void OnOkClick(object sender, EventArgs e) {
            DialogResult res = XtraMessageBox.Show("Are you shure, that all parameters are ok?", "Adding Trailing", MessageBoxButtons.YesNo);
            if(res != DialogResult.Yes)
                return;
            DialogResult = DialogResult.OK;
            UpdateOrAddTrailing();
            Close();
        }
        private static readonly object accepted = new object();
        public event EventHandler Accepted {
            add { Events.AddHandler(accepted, value); }
            remove { Events.RemoveHandler(accepted, value); }
        }
        void UpdateOrAddTrailing() {
            EventHandler handler = Events[accepted] as EventHandler;
            if(handler != null)
                handler.Invoke(this, EventArgs.Empty);
        }
    }
}
