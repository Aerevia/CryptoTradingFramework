﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoMarketClient.Bittrex;
using CryptoMarketClient.Poloniex;

namespace CryptoMarketClient.Common {
    public class DepositInfo {
        public string HostName { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal RateInBTC { get; set; }
        public decimal RateInUSD { get; set; }
    }

    public class DepositInfoHelper {
        static DepositInfoHelper defaultHelper;
        public static DepositInfoHelper Default {
            get {
                if(defaultHelper == null)
                    defaultHelper = new DepositInfoHelper();
                return defaultHelper;
            }
        }

        public void FillDepositTotalBittrex(List<DepositInfo> list) {
            if(!BittrexModel.Default.UpdateBalances()) {
                LogManager.Default.AddError("FillDepositTotalBittrex: can't get balances");
                return;
            }

            if(!BittrexModel.Default.UpdateTickersInfo()) {
                LogManager.Default.AddError("FillDepositTotalBittrex: can't get markets info");
                return;
            }

            BittrexTicker btcMarket = (BittrexTicker)BittrexModel.Default.Tickers.FirstOrDefault(m => m.BaseCurrency == "USDT" && m.MarketCurrency == "BTC");
            if(btcMarket == null) {
                LogManager.Default.AddError("FillDepositTotalBittrex: can't get BTC currency info");
                return;
            }
            

            foreach(BittrexAccountBalanceInfo info in BittrexModel.Default.Balances) {
                if(info.Available == 0)
                    continue;
                DepositInfo dep = new DepositInfo();
                dep.HostName = PoloniexModel.Default.Name;
                dep.Currency = info.Currency;
                dep.Amount = info.Available;

                if(dep.Currency != "BTC") {
                    BittrexTicker market = (BittrexTicker)BittrexModel.Default.Tickers.FirstOrDefault(m => m.MarketCurrency == dep.Currency);
                    if(market == null) {
                        LogManager.Default.AddWarning("FillDepositTotalBittrex: can't get " + dep.Currency + " currency info");
                        continue;
                    }
                    dep.RateInBTC = dep.Amount * market.Last;
                }
                else {
                    dep.RateInBTC = info.Available;
                }
                dep.RateInUSD = dep.RateInBTC * btcMarket.Last;
                list.Add(dep);
            }
        }
        public void FillDepositTotalPoloniex(List<DepositInfo> list) {
            if(!PoloniexModel.Default.UpdateBalances()) {
                LogManager.Default.AddError("FillDepositTotalPoloniex: can't get balances");
                return;
            }

            if(!PoloniexModel.Default.UpdateTickersInfo()) {
                LogManager.Default.AddError("FillDepositTotalPoloniex: can't get update markets info");
                return;
            }

            PoloniexTicker btcMarket = (PoloniexTicker)PoloniexModel.Default.Tickers.FirstOrDefault(m => m.BaseCurrency == "USDT" && m.MarketCurrency == "BTC");
            if(btcMarket == null) {
                LogManager.Default.AddError("FillDepositTotalPoloniex: can't get BTC currency info");
                return;
            }

            foreach(PoloniexAccountBalanceInfo info in PoloniexModel.Default.Balances) {
                if(info.Available == 0)
                    continue;
                DepositInfo dep = new DepositInfo();
                dep.HostName = BittrexModel.Default.Name;
                dep.Currency = info.Currency;
                dep.Amount = info.Available;

                if(dep.Currency != "BTC") {
                    PoloniexTicker market = (PoloniexTicker)PoloniexModel.Default.Tickers.FirstOrDefault(m => m.MarketCurrency == dep.Currency);
                    if(market == null) {
                        LogManager.Default.AddError("FillDepositTotalPoloniex: can't get " + dep.Currency + " currency info");
                        continue;
                    }
                    dep.RateInBTC = dep.Amount * market.Last;
                }
                else {
                    dep.RateInBTC = info.Available;
                }
                dep.RateInUSD = dep.RateInBTC * btcMarket.Last;
                list.Add(dep);
            }
        }
    }
}
