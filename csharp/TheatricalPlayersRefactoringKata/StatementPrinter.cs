using System;
using System.Collections.Generic;
using System.Globalization;
using TheatricalPlayersRefactoringKata;

namespace TheatricalPlayersRefactoringKata
{
    public class StatementPrinter
    {
        public string Print(Invoice invoice, Dictionary<string, Play> plays)
        {
            var values = new InvoiceValues(invoice, plays);
            var totalAmount = 0;
            var volumeCredits = 0;
            var result = string.Format("Statement for {0}\n", invoice.Customer);
            CultureInfo cultureInfo = new CultureInfo("en-US");

            foreach(var perf in invoice.Performances) 
            {
                var play = plays[perf.PlayID];
                var thisAmount = values.CalculateAmount(play, perf);
                
                // add volume credits
                volumeCredits += Math.Max(perf.Audience - 30, 0);
                values.Credits = volumeCredits;
                // add extra credit for every ten comedy attendees
                if ("comedy" == play.Type) volumeCredits += (int)Math.Floor((decimal)perf.Audience / 5);

                // print line for this order
                result += String.Format(cultureInfo, "  {0}: {1:C} ({2} seats)\n", play.Name, Convert.ToDecimal(thisAmount / 100), perf.Audience);
                totalAmount += thisAmount;
                values.Total = totalAmount;
            }
            result += String.Format(cultureInfo, "Amount owed is {0:C}\n", Convert.ToDecimal(values.Total / 100));
            result += String.Format("You earned {0} credits\n", values.Credits);
            return result;
        }
    }
}

public class InvoiceValues
{
    public decimal Total { get; set; }
    public int Credits { get; set; }
    public string Header { get; set; }
    
    private readonly Invoice _invoice;
    private readonly Dictionary<string, Play> _plays;

    public InvoiceValues(Invoice invoice, Dictionary<string, Play> plays)
    {
        _invoice = invoice;
        _plays = plays;
    }

    public int CalculateAmount(Play play, Performance perf)
    {
        var thisAmount = 0;
        switch (play.Type) 
        {
            case "tragedy":
                thisAmount = 40000;
                if (perf.Audience > 30) {
                    thisAmount += 1000 * (perf.Audience - 30);
                }
                break;
            case "comedy":
                thisAmount = 30000;
                if (perf.Audience > 20) {
                    thisAmount += 10000 + 500 * (perf.Audience - 20);
                }
                thisAmount += 300 * perf.Audience;
                break;
            default:
                throw new Exception("unknown type: " + play.Type);
        }

        return thisAmount;
    }
}

