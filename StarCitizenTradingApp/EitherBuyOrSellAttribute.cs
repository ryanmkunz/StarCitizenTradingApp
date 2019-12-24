using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StarCitizenTradingApp
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EitherBuyOrSellAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var typeInfo = value.GetType();
            var propertyInfo = typeInfo.GetProperties();

            var purchaseCost = propertyInfo.FirstOrDefault(p => p.Name == "PurchaseCost").GetValue(value, null);
            var sellPrice = propertyInfo.FirstOrDefault(p => p.Name == "SellPrice").GetValue(value, null);

            if (purchaseCost.Equals(0.00) && !sellPrice.Equals(0.00))
            {
                return true;
            }
            if (sellPrice.Equals(0.00) && !purchaseCost.Equals(0.00))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}