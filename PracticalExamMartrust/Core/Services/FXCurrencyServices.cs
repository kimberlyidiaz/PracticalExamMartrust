using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PracticalExamMartrust.Core.DTOs;
using PracticalExamMartrust.Core.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
namespace PracticalExamMartrust.Core.Services
{
    public class FXCurrencyServices
    {
        public List<Currencies> GetCurrencyList()
        {
            List<Currencies> currencies = new List<Currencies>();

            //Call API from https://exchangeratesapi.io/ for currencies
            string ApiURL = "http://api.exchangeratesapi.io/v1/latest?access_key=042e1224c4e1edf1cc5ec63ce2b80e2f";

            var client = new RestClient(ApiURL);
            var request = new RestRequest(Method.GET);

            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {

                try
                {
                    string jsonContent = String.Format("[{0}]", response.Content);
                    var JSON = JsonConvert.DeserializeObject<JArray>(jsonContent);

                    dynamic data = JObject.Parse(response.Content);

                    foreach (var item in data.rates)
                    {
                        string a = item.Name.ToString();
                        var b = item.Value.Value;
                        string test = string.Empty;
                        Currencies cur = new Currencies { CurrencyCode = a, CurrencyAmount = b };
                        currencies.Add(cur);
                    }
                }
                catch (Exception ex)
                {
                    currencies = default;
                }

            }

            return currencies;
        }

        public FXCurrencyDTO MyProperty(string buyCurrency, string buyAmount, string sellCurrency, string sellAmount, string mode)
        {
            FXCurrencyDTO response = new FXCurrencyDTO();
            var hasSellingCurrency = new Currencies();
            var hasBuyingCurrency = new Currencies();
            #region validations
            double amount;

            if (mode == "1")
            {
                if (!String.IsNullOrEmpty(buyAmount))
                {
                    if (Convert.ToDouble(buyAmount) > 0)
                    {
                        bool isBuyAmtDouble = Double.TryParse(buyAmount, out amount);
                        if (!isBuyAmtDouble)
                        {
                            // double here
                            return new FXCurrencyDTO
                            {
                                Message = "Invalid Buying Amount."
                            };
                        }
                    }
                }
            }
            else if (mode == "2")
            {
                if (!String.IsNullOrEmpty(sellAmount))
                {
                    if (Convert.ToDouble(sellAmount) > 0)
                    {
                        bool isSellAmtDouble = Double.TryParse(sellAmount, out amount);
                        if (!isSellAmtDouble)
                        {
                            // double here
                            return new FXCurrencyDTO
                            {
                                Message = "Invalid Selling Amount."

                            };
                        }
                    }

                }
            }

            //check if currency exist
            var currencies = this.GetCurrencyList();

            if (!String.IsNullOrEmpty(buyCurrency))
            {
                hasBuyingCurrency = currencies.Where(x => x.CurrencyCode.ToLower() == buyCurrency.ToLower()).FirstOrDefault();

                if (hasBuyingCurrency == null)
                    response.Message = "Invalid Buying Currency.";
            }
            else
            {
                response.Message = "Invalid Buying Currency.";
            }

            if (!String.IsNullOrEmpty(buyCurrency))
            {
                hasSellingCurrency = currencies.Where(x => x.CurrencyCode.ToLower() == sellCurrency.ToLower()).FirstOrDefault();
                if (hasSellingCurrency == null)
                    response.Message = "Invalid Selling Currency.";
            }
            else
            {
                response.Message = "Invalid Selling Currency.";
            }
            #endregion


            try
            {
                if (String.IsNullOrEmpty(response.Message))
                {
                    //buying
                    if (mode == "1")
                    {
                        response.Message = "Success";
                        response.Value = hasBuyingCurrency.CurrencyAmount * Convert.ToDouble(buyAmount);
                        response.Currency = "1 - " + hasBuyingCurrency.CurrencyCode + " = " + hasBuyingCurrency.CurrencyAmount + " " + sellCurrency;
                    }
                    //selling
                    else if (mode == "2")
                    {
                        response.Message = "Success";
                        response.Value = Convert.ToDouble(sellAmount) / hasSellingCurrency.CurrencyAmount;
                        response.Currency = "1 - " + hasSellingCurrency.CurrencyCode + " = " + hasSellingCurrency.CurrencyAmount + " " + buyCurrency;
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                response.Message = "Error";
                return response;
            }

        }
    }
}
