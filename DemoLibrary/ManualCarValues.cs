namespace DemoLibrary
{
    // Loads manual values for cars like price and type
    internal class ManualCarValues
    {
        internal string Name { get; set; }
        internal CarGroup Group { get; set; }
        internal int Price { get; set; }
        internal ManualCarValues(string name, CarGroup group, int price)
        {
            Name = name;
            Group = group;
            Price = price;
        }


        internal static List<ManualCarValues> LoadCarValues()
        {
            List<ManualCarValues> output = new List<ManualCarValues>
            {
                new ManualCarValues("Toyota Sera", CarGroup.Stock, 50000),
                new ManualCarValues("Giulietta QV Launch Edition 2014", CarGroup.Stock, 10000),
                new ManualCarValues("Ferrari F40", CarGroup.Stock, 80000),
                new ManualCarValues("Abarth 595 SS", CarGroup.Stock, 10000),
                new ManualCarValues("Abarth 595 SS Step 1", CarGroup.Stock, 10000),
                new ManualCarValues("Abarth 595 SS Step 2", CarGroup.Stock, 10000),
                new ManualCarValues("Alfa Romeo Mito QV", CarGroup.Stock, 10000),
                new ManualCarValues("Alfa Romeo GTA", CarGroup.Stock, 10000),
                new ManualCarValues("Ferrari 250 GTO", CarGroup.Stock, 10000),
                new ManualCarValues("Ferrari GTO", CarGroup.Stock, 10000),
                new ManualCarValues("Lamborghini Miura P400 SV", CarGroup.Stock, 10000),
                new ManualCarValues("Mazda Miata NA", CarGroup.Stock, 10000),
                new ManualCarValues("Toyota Supra MKIV", CarGroup.Stock, 10000),
                new ManualCarValues("Volkswagen Beetle 1600s Type B", CarGroup.Stock, 10000),

                new ManualCarValues("Toyota Sera GT", CarGroup.GT, 700000),
                new ManualCarValues("Toyota Sera TRD", CarGroup.GT, 700000),
                new ManualCarValues("Abarth 500 EsseEsse", CarGroup.GT, 700000),
                new ManualCarValues("Abarth 500 EsseEsse Step1", CarGroup.GT, 700000),
                new ManualCarValues("BMW M3 E30 Gr.A 92", CarGroup.GT, 700000),
                new ManualCarValues("BMW M3 E30 Group A", CarGroup.GT, 700000),
                new ManualCarValues("BMW M3 GT2", CarGroup.GT, 700000),
                new ManualCarValues("Ferrari 599XX EVO", CarGroup.GT, 700000),
                new ManualCarValues("Ferrari F40 Stage 3", CarGroup.GT, 700000),
                new ManualCarValues("Volkswagen Fusca 1800", CarGroup.GT, 700000),
                new ManualCarValues("Abarth 500 Assetto Corse", CarGroup.GT, 700000),
                new ManualCarValues("Alfa Romeo 155 TI V6", CarGroup.GT, 700000),
                new ManualCarValues("Audi R18 e-tron quattro 2014", CarGroup.GT, 700000),
                new ManualCarValues("Audi R8 LMS Ultra", CarGroup.GT, 700000),
                new ManualCarValues("Audi Sport quattro", CarGroup.GT, 700000),
                new ManualCarValues("Chevrolet Corvette C7R", CarGroup.GT, 700000),
                new ManualCarValues("Ford Escort RS1600", CarGroup.GT, 700000),
                new ManualCarValues("Lamborghini Huracan GT3", CarGroup.GT, 700000),
                new ManualCarValues("Lotus 3-Eleven", CarGroup.GT, 700000),
                new ManualCarValues("Mazda MX5 Cup", CarGroup.GT, 700000),
                new ManualCarValues("Mazda MX5 ND", CarGroup.GT, 700000),
                new ManualCarValues("Mazda RX-7 Spirit R", CarGroup.GT, 700000),
                new ManualCarValues("Mazda RX-7 Tuned", CarGroup.GT, 700000),
                new ManualCarValues("McLaren 650S GT3", CarGroup.GT, 700000),
                new ManualCarValues("McLaren F1 GTR", CarGroup.GT, 700000),
                new ManualCarValues("Mercedes-Benz 190E EVO II", CarGroup.GT, 700000),
                new ManualCarValues("Mercedes-Benz AMG GT3", CarGroup.GT, 700000),
                new ManualCarValues("Porsche 718 Cayman S", CarGroup.GT, 700000),
                new ManualCarValues("Porsche 911 GT3 Cup 2017", CarGroup.GT, 700000),
                new ManualCarValues("Porsche 911 GT3 RS", CarGroup.GT, 700000),
                new ManualCarValues("Porsche 911 GT3 R 2016", CarGroup.GT, 700000),
                new ManualCarValues("Porsche 919 Hybrid 2015", CarGroup.GT, 700000),
                new ManualCarValues("Porsche 919 Hybrid 2016", CarGroup.GT, 700000),
                new ManualCarValues("Praga R1", CarGroup.GT, 700000),
                new ManualCarValues("Toyota AE86 Drift", CarGroup.GT, 700000),
                new ManualCarValues("Toyota AE86 Tuned", CarGroup.GT, 700000),
                new ManualCarValues("Toyota Supra MKIV Drift", CarGroup.GT, 700000),
                new ManualCarValues("Toyota Supra MKIV Time Attack", CarGroup.GT, 700000),
                new ManualCarValues("Toyota TS040 Hybrid 2014", CarGroup.GT, 700000),
                new ManualCarValues("KTM X-Bow R", CarGroup.GT, 700000),
                new ManualCarValues("Lotus 2-Eleven", CarGroup.GT, 700000),
                new ManualCarValues("Lotus 2-Eleven GT4", CarGroup.GT, 700000),
                new ManualCarValues("Lotus Type 49", CarGroup.Vintage, 700000),
                new ManualCarValues("Lotus Evora GTC", CarGroup.GT, 700000),
                new ManualCarValues("Lotus Evora GTE", CarGroup.GT, 700000),
                new ManualCarValues("Mercedes SLS AMG GT3", CarGroup.GT, 700000),
                new ManualCarValues("Radical SR3 LHD", CarGroup.GT, 700000),
                new ManualCarValues("Formula RSS 2013 V8", CarGroup.F1, 700000),
                new ManualCarValues("Formula Americas 2020", CarGroup.F1, 700000),
                new ManualCarValues("GT-M Bayro 6 V8", CarGroup.GT, 700000),
                new ManualCarValues("GT-M Lanzo V10", CarGroup.GT, 700000),
                new ManualCarValues("GT-N Darche 96 F6", CarGroup.GT, 700000),
                new ManualCarValues("GT-N Ferruccio 36 V8", CarGroup.GT, 700000),
                new ManualCarValues("GT Ferruccio 55 V12", CarGroup.GT, 700000),
                new ManualCarValues("GT Ferruccio 57 V12", CarGroup.GT, 700000),
                new ManualCarValues("GT Lanzo V12", CarGroup.GT, 700000),
                new ManualCarValues("GT Shadow V8", CarGroup.GT, 700000),
                new ManualCarValues("GT Tornado V12", CarGroup.GT, 700000),
                new ManualCarValues("GT Vortex V10", CarGroup.GT, 700000),
                new ManualCarValues("LMGT Nisumo R39 V8", CarGroup.GT, 700000),
                new ManualCarValues("LMGT Toyama 2-Zero V8", CarGroup.GT, 700000),
                new ManualCarValues("LMP1 Protech P91 Hybrid Evo", CarGroup.GT, 700000),
                new ManualCarValues("LMP Ferruccio 33 V12", CarGroup.GT, 700000),
                new ManualCarValues("Lamborghini Murcielago SGT300", CarGroup.GT, 700000),
                new ManualCarValues("SCG 007LMH", CarGroup.GT, 700000),
                new ManualCarValues("Volkswagen Beetle 1600s D-Spec", CarGroup.GT, 100000),
                new ManualCarValues("SuperEscarabajo", CarGroup.GT, 300000),
                new ManualCarValues("BMW E30 Touring Winter Drift", CarGroup.GT, 700000),
                new ManualCarValues("Nissan Laurel C33 Winter Drift", CarGroup.GT, 700000),
                new ManualCarValues("Nissan Silvia S13 Winter Drift", CarGroup.GT, 700000),

                new ManualCarValues("Tatuus FA01", CarGroup.Formula, 300000),
                new ManualCarValues("RedBull X2010", CarGroup.Formula, 8000000),
                new ManualCarValues("RedBull X2010 S1", CarGroup.Formula, 200000000),
                new ManualCarValues("Formula Super Vee Uruguay", CarGroup.Formula, 130000),
                new ManualCarValues("Dallara F312", CarGroup.Formula, 400000),
                new ManualCarValues("Ferrari F2004", CarGroup.F1, 4000000),
                new ManualCarValues("Ferrari SF15-T", CarGroup.F1, 5000000),
                new ManualCarValues("Ferrari SF70H", CarGroup.F1, 5000000),
                new ManualCarValues("Formula Hybrid 2022", CarGroup.F1, 40000000),
                new ManualCarValues("Formula Hybrid 2022 S", CarGroup.F1, 40000000),
                new ManualCarValues("Formula RSS 2 V6 2020", CarGroup.Formula, 4000000),
                new ManualCarValues("Formula RSS 4", CarGroup.Formula, 250000),

                new ManualCarValues("Kart 50", CarGroup.Kart, 5000),
                new ManualCarValues("Kart 100", CarGroup.Kart, 7000),
                new ManualCarValues("Kart 125 Shifter", CarGroup.Kart, 10000),
                new ManualCarValues("Superkart 250cc", CarGroup.Kart, 50000),

                new ManualCarValues("Ferrari 312/67", CarGroup.Vintage, 5000000),
                new ManualCarValues("Ferrari 330 P4", CarGroup.Vintage, 5000000),
                new ManualCarValues("Lotus Type 25", CarGroup.Vintage, 1000000),
                new ManualCarValues("Maserati 250F 12 cylinder", CarGroup.Vintage, 10000),
                new ManualCarValues("Maserati 250F 6 cylinder", CarGroup.Vintage, 10000),
                new ManualCarValues("Mazda 787B", CarGroup.Vintage, 1200000),
                new ManualCarValues("Mercedes-Benz C9 1989 LM", CarGroup.Vintage, 1000000),
                new ManualCarValues("Porsche 718 RS 60 Spyder", CarGroup.Vintage, 1600000),
                new ManualCarValues("Porsche 908 LH", CarGroup.Vintage, 1500000),
                new ManualCarValues("Porsche 911 GT1-98", CarGroup.Vintage, 700000),
                new ManualCarValues("Porsche 917/30 Spyder", CarGroup.Vintage, 1200000),
                new ManualCarValues("Porsche 917 K", CarGroup.Vintage, 700000),
                new ManualCarValues("Porsche 918 Spyder", CarGroup.Vintage, 1200000),
                new ManualCarValues("RUF CTR Yellowbird", CarGroup.Vintage, 200000),
                new ManualCarValues("Shelby Cobra 427 S/C", CarGroup.Vintage, 100000),
                new ManualCarValues("Porsche 935/78 'Moby Dick'", CarGroup.Vintage, 140000),
                new ManualCarValues("Porsche 962 C Long Tail", CarGroup.Vintage, 1000000),
                new ManualCarValues("Formula RSS 1990 V12", CarGroup.Vintage, 5000000),
                
                new ManualCarValues("Formula Americas 2020 Oval", CarGroup.Oval, 10000),
                new ManualCarValues("Hyperion 2020", CarGroup.Oval, 10000),

                
                
                
            };

            return output;
        }
    }
}
