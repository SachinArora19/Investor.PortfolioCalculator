﻿public class Investment
{
    public string InvestorId { get; set; }
    public string InvestmentId { get; set; }
    public InvestmentType InvestmentType { get; set; }
    public string? ISIN { get; set; }
    public string? City { get; set; }
    public string? FondsInvestor { get; set; }
}
