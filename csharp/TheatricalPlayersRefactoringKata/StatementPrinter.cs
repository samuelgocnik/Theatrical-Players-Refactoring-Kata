using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TheatricalPlayersRefactoringKata;

namespace TheatricalPlayersRefactoringKata
{
    public class StatementPrinter
    {
        public string Print(InvoiceValues values)
        {
            var result = string.Format("Statement for {0}\n", values.CustomerName);
            CultureInfo cultureInfo = new CultureInfo("en-US");
            foreach (var currPlay in values.CurrentPlay)
            {
                result += String.Format(cultureInfo, "  {0}: {1:C} ({2} seats)\n", currPlay.Name, Convert.ToDecimal(currPlay.Amount), currPlay.Seats);
            }
            result += String.Format(cultureInfo, "Amount owed is {0:C}\n", Convert.ToDecimal(values.Total / 100));
            result += String.Format("You earned {0} credits\n", values.Credits);
            return result;
        }
        
        public string PrintHtml(InvoiceValues values)
        {
            StringBuilder result = new StringBuilder();
            result.Append("<html>\n");
            result.Append("<body>\n");
            result.Append(string.Format("<h1>Statement for {0}</h1>\n", values.CustomerName));
            result.Append("<ul>\n");
            result.Append("<li>Seats:</li>\n");
            CultureInfo cultureInfo = new CultureInfo("en-US");
            foreach (var currPlay in values.CurrentPlay)
            {
                result.Append(String.Format(cultureInfo, "  {0}: {1:C} ({2} seats)\n", currPlay.Name, Convert.ToDecimal(currPlay.Amount), currPlay.Seats));
            }
            result.Append(String.Format(cultureInfo, "Amount owed is {0:C}\n", Convert.ToDecimal(values.Total / 100)));
            result.Append(String.Format("You earned {0} credits\n", values.Credits));
            result.Append("</body>\n");
            result.Append("</html>\n");
            return result.ToString();
        }
    }
}

public class CurrentPlay {    
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public int Seats { get; set; }

}

public class InvoiceValues
{
    public decimal Total { get; set; }
    public int Credits { get; set; }
    public string CustomerName { get; set; }
    public List<CurrentPlay> CurrentPlay { get; set; }

    private readonly Invoice _invoice;
    private readonly Dictionary<string, Play> _plays;

    public InvoiceValues(Invoice invoice, Dictionary<string, Play> plays)
    {
        _invoice = invoice;
        _plays = plays;
        CurrentPlay = new List<CurrentPlay>();
        CustomerName = invoice.Customer;
        PreparePlays();
    }

    private int CalculateAmount(Play play, Performance perf)
    {
        int thisAmount;
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

    private void CalculateVolumeCredits(Play play, Performance perf)
    {
        Credits += Math.Max(perf.Audience - 30, 0);
        // add extra credit for every ten comedy attendees
        if ("comedy" == play.Type) Credits += (int)Math.Floor((decimal)perf.Audience / 5);
    }

    private void PreparePlays()
    {
        foreach(var perf in _invoice.Performances)
        {
            var play = _plays[perf.PlayID];
            var thisAmount = CalculateAmount(play, perf);
            CalculateVolumeCredits(play, perf);
            CurrentPlay.Add(new CurrentPlay { Name = play.Name, Amount = Convert.ToDecimal(thisAmount / 100), Seats = perf.Audience });;
            Total += thisAmount;
        }
    }
}


