

namespace DBLink
{
    // Loads manual values for cars like price and type
    internal class ManualCarData
    {
        internal string Name { get; set; }
        internal CarGroup Group { get; set; }
        internal int Price { get; set; }
        internal ManualCarData(string name, CarGroup group, int price)
        {
            Name = name;
            Group = group;
            Price = price;
        }


        internal static List<ManualCarData> LoadCarValues()
        {
            List<ManualCarData> output = new List<ManualCarData>
            {
                new ManualCarData("Toyota Sera", CarGroup.Stock, 50000),
                new ManualCarData("Giulietta QV Launch Edition 2014", CarGroup.Stock, 10000),
                new ManualCarData("Ferrari F40", CarGroup.Stock, 80000),
                new ManualCarData("Abarth 595 SS", CarGroup.Stock, 10000),
                new ManualCarData("Abarth 595 SS Step 1", CarGroup.Stock, 10000),
                new ManualCarData("Abarth 595 SS Step 2", CarGroup.Stock, 10000),
                new ManualCarData("Alfa Romeo Mito QV", CarGroup.Stock, 10000),
                new ManualCarData("Alfa Romeo GTA", CarGroup.Stock, 10000),
                new ManualCarData("Ferrari 250 GTO", CarGroup.Stock, 10000),
                new ManualCarData("Ferrari GTO", CarGroup.Stock, 10000),
                new ManualCarData("Lamborghini Miura P400 SV", CarGroup.Stock, 10000),
                new ManualCarData("Mazda Miata NA", CarGroup.Stock, 10000),
                new ManualCarData("Toyota Supra MKIV", CarGroup.Stock, 10000),
                new ManualCarData("Volkswagen Beetle 1600s Type B", CarGroup.Stock, 10000),

                new ManualCarData("Toyota Sera GT", CarGroup.GT, 700000),
                new ManualCarData("Toyota Sera TRD", CarGroup.GT, 700000),
                new ManualCarData("Abarth 500 EsseEsse", CarGroup.GT, 700000),
                new ManualCarData("Abarth 500 EsseEsse Step1", CarGroup.GT, 700000),
                new ManualCarData("BMW M3 E30 Gr.A 92", CarGroup.GT, 700000),
                new ManualCarData("BMW M3 E30 Group A", CarGroup.GT, 700000),
                new ManualCarData("BMW M3 GT2", CarGroup.GT, 700000),
                new ManualCarData("Ferrari 599XX EVO", CarGroup.GT, 700000),
                new ManualCarData("Ferrari F40 Stage 3", CarGroup.GT, 700000),
                new ManualCarData("Volkswagen Fusca 1800", CarGroup.GT, 700000),
                new ManualCarData("Abarth 500 Assetto Corse", CarGroup.GT, 700000),
                new ManualCarData("Alfa Romeo 155 TI V6", CarGroup.GT, 700000),
                new ManualCarData("Audi R18 e-tron quattro 2014", CarGroup.GT, 700000),
                new ManualCarData("Audi R8 LMS Ultra", CarGroup.GT, 700000),
                new ManualCarData("Audi Sport quattro", CarGroup.GT, 700000),
                new ManualCarData("Chevrolet Corvette C7R", CarGroup.GT, 700000),
                new ManualCarData("Ford Escort RS1600", CarGroup.GT, 700000),
                new ManualCarData("Lamborghini Huracan GT3", CarGroup.GT, 700000),
                new ManualCarData("Lotus 3-Eleven", CarGroup.GT, 700000),
                new ManualCarData("Mazda MX5 Cup", CarGroup.GT, 700000),
                new ManualCarData("Mazda MX5 ND", CarGroup.GT, 700000),
                new ManualCarData("Mazda RX-7 Spirit R", CarGroup.GT, 700000),
                new ManualCarData("Mazda RX-7 Tuned", CarGroup.GT, 700000),
                new ManualCarData("McLaren 650S GT3", CarGroup.GT, 700000),
                new ManualCarData("McLaren F1 GTR", CarGroup.GT, 700000),
                new ManualCarData("Mercedes-Benz 190E EVO II", CarGroup.GT, 700000),
                new ManualCarData("Mercedes-Benz AMG GT3", CarGroup.GT, 700000),
                new ManualCarData("Porsche 718 Cayman S", CarGroup.GT, 700000),
                new ManualCarData("Porsche 911 GT3 Cup 2017", CarGroup.GT, 700000),
                new ManualCarData("Porsche 911 GT3 RS", CarGroup.GT, 700000),
                new ManualCarData("Porsche 911 GT3 R 2016", CarGroup.GT, 700000),
                new ManualCarData("Porsche 919 Hybrid 2015", CarGroup.GT, 700000),
                new ManualCarData("Porsche 919 Hybrid 2016", CarGroup.GT, 700000),
                new ManualCarData("Praga R1", CarGroup.GT, 300000),
                new ManualCarData("Toyota AE86 Drift", CarGroup.GT, 700000),
                new ManualCarData("Toyota AE86 Tuned", CarGroup.GT, 700000),
                new ManualCarData("Toyota Supra MKIV Drift", CarGroup.GT, 700000),
                new ManualCarData("Toyota Supra MKIV Time Attack", CarGroup.GT, 700000),
                new ManualCarData("Toyota TS040 Hybrid 2014", CarGroup.GT, 700000),
                new ManualCarData("KTM X-Bow R", CarGroup.GT, 700000),
                new ManualCarData("Lotus 2-Eleven", CarGroup.GT, 700000),
                new ManualCarData("Lotus 2-Eleven GT4", CarGroup.GT, 700000),
                new ManualCarData("Lotus Type 49", CarGroup.Vintage, 700000),
                new ManualCarData("Lotus Evora GTC", CarGroup.GT, 700000),
                new ManualCarData("Lotus Evora GTE", CarGroup.GT, 700000),
                new ManualCarData("Mercedes SLS AMG GT3", CarGroup.GT, 700000),
                new ManualCarData("Radical SR3 LHD", CarGroup.GT, 200000),
                new ManualCarData("Formula RSS 2013 V8", CarGroup.F1, 700000),
                new ManualCarData("Formula Americas 2020", CarGroup.F1, 700000),
                new ManualCarData("GT-M Bayro 6 V8", CarGroup.GT, 700000),
                new ManualCarData("GT-M Lanzo V10", CarGroup.GT, 700000),
                new ManualCarData("GT-N Darche 96 F6", CarGroup.GT, 700000),
                new ManualCarData("GT-N Ferruccio 36 V8", CarGroup.GT, 700000),
                new ManualCarData("GT Ferruccio 55 V12", CarGroup.GT, 700000),
                new ManualCarData("GT Ferruccio 57 V12", CarGroup.GT, 700000),
                new ManualCarData("GT Lanzo V12", CarGroup.GT, 700000),
                new ManualCarData("GT Shadow V8", CarGroup.GT, 700000),
                new ManualCarData("GT Tornado V12", CarGroup.GT, 700000),
                new ManualCarData("GT Vortex V10", CarGroup.GT, 700000),
                new ManualCarData("LMGT Nisumo R39 V8", CarGroup.GT, 700000),
                new ManualCarData("LMGT Toyama 2-Zero V8", CarGroup.GT, 700000),
                new ManualCarData("LMP1 Protech P91 Hybrid Evo", CarGroup.GT, 80000000),
                new ManualCarData("LMP Ferruccio 33 V12", CarGroup.GT, 700000),
                new ManualCarData("Lamborghini Murcielago SGT300", CarGroup.GT, 700000),
                new ManualCarData("SCG 007LMH", CarGroup.GT, 700000),
                new ManualCarData("Volkswagen Beetle 1600s D-Spec", CarGroup.GT, 100000),
                new ManualCarData("SuperEscarabajo", CarGroup.GT, 300000),
                new ManualCarData("BMW E30 Touring Winter Drift", CarGroup.GT, 700000),
                new ManualCarData("Nissan Laurel C33 Winter Drift", CarGroup.GT, 700000),
                new ManualCarData("Nissan Silvia S13 Winter Drift", CarGroup.GT, 700000),

                new ManualCarData("Tatuus FA01", CarGroup.Formula, 300000),
                new ManualCarData("RedBull X2010", CarGroup.Formula, 8000000),
                new ManualCarData("RedBull X2010 S1", CarGroup.Formula, 200000000),
                new ManualCarData("Formula Super Vee Uruguay", CarGroup.Formula, 130000),
                new ManualCarData("Dallara F312", CarGroup.Formula, 400000),
                new ManualCarData("Ferrari F2004", CarGroup.F1, 4000000),
                new ManualCarData("Ferrari SF15-T", CarGroup.F1, 5000000),
                new ManualCarData("Ferrari SF70H", CarGroup.F1, 5000000),
                new ManualCarData("Formula Hybrid 2022", CarGroup.F1, 40000000),
                new ManualCarData("Formula Hybrid 2022 S", CarGroup.F1, 40000000),
                new ManualCarData("Formula RSS 2 V6 2020", CarGroup.Formula, 4000000),
                new ManualCarData("Formula RSS 4", CarGroup.Formula, 250000),
                new ManualCarData("Formula Hybrid® 2023", CarGroup.F1, 25000000),

                new ManualCarData("Kart 50", CarGroup.Kart, 5000),
                new ManualCarData("Kart 100", CarGroup.Kart, 7000),
                new ManualCarData("Kart 125 Shifter", CarGroup.Kart, 10000),
                new ManualCarData("Superkart 250cc", CarGroup.Kart, 50000),

                new ManualCarData("Ferrari 312/67", CarGroup.Vintage, 5000000),
                new ManualCarData("Ferrari 330 P4", CarGroup.Vintage, 5000000),
                new ManualCarData("Lotus Type 25", CarGroup.Vintage, 1000000),
                new ManualCarData("Maserati 250F 12 cylinder", CarGroup.Vintage, 10000),
                new ManualCarData("Maserati 250F 6 cylinder", CarGroup.Vintage, 10000),
                new ManualCarData("Mazda 787B", CarGroup.Vintage, 1200000),
                new ManualCarData("Mercedes-Benz C9 1989 LM", CarGroup.Vintage, 1000000),
                new ManualCarData("Porsche 718 RS 60 Spyder", CarGroup.Vintage, 1600000),
                new ManualCarData("Porsche 908 LH", CarGroup.Vintage, 1500000),
                new ManualCarData("Porsche 911 GT1-98", CarGroup.Vintage, 700000),
                new ManualCarData("Porsche 917/30 Spyder", CarGroup.Vintage, 1200000),
                new ManualCarData("Porsche 917 K", CarGroup.Vintage, 700000),
                new ManualCarData("Porsche 918 Spyder", CarGroup.Vintage, 1200000),
                new ManualCarData("RUF CTR Yellowbird", CarGroup.Vintage, 200000),
                new ManualCarData("Shelby Cobra 427 S/C", CarGroup.Vintage, 100000),
                new ManualCarData("Porsche 935/78 'Moby Dick'", CarGroup.Vintage, 140000),
                new ManualCarData("Porsche 962 C Long Tail", CarGroup.Vintage, 1000000),
                new ManualCarData("Formula RSS 1990 V12", CarGroup.Vintage, 5000000),
                new ManualCarData("Formula RSS 1970 V8", CarGroup.Vintage, 5000000),
                new ManualCarData("Formula RSS 1970 V8 S", CarGroup.Vintage, 5000000),

                new ManualCarData("Formula Americas 2020 Oval", CarGroup.Oval, 10000),
                new ManualCarData("Hyperion 2020", CarGroup.Oval, 10000),

                
                
                
            };

            return output;
        }
    }
}
